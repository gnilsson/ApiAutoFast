using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;

namespace ApiAutoFast.SourceGenerator;

internal static class Parser
{
    private static readonly string[] _propertyTargetAttributeNames = Enum.GetNames(typeof(AttributeModelTargetType));

    private const string AutoFastEndpointsAttribute = "ApiAutoFast.AutoFastEndpointsAttribute";
    private const string AutoFastContextAttribute = "ApiAutoFast.AutoFastContextAttribute";

    internal static SemanticTargetInformation? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

        foreach (var attributeListSyntax in classDeclarationSyntax.AttributeLists)
        {
            foreach (var attributeSyntax in attributeListSyntax.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                {
                    continue;
                }

                var attributeContainingTypeSymbol = attributeSymbol.ContainingType;

                var fullName = attributeContainingTypeSymbol.ToDisplayString();

                if (fullName is AutoFastEndpointsAttribute or AutoFastContextAttribute)
                {
                    return new(classDeclarationSyntax, fullName);
                }
            }
        }

        return null;
    }

    internal static bool IsSyntaxTargetForGeneration(SyntaxNode node) => node is ClassDeclarationSyntax m && m.AttributeLists.Count > 0;

    internal static GenerationConfig? GetGenerationConfig(Compilation compilation, IEnumerable<SemanticTargetInformation> semanticTargetInformations, CancellationToken ct)
    {
        var endpointAttribute = compilation.GetTypeByMetadataName(AutoFastEndpointsAttribute);
        var contextAttribute = compilation.GetTypeByMetadataName(AutoFastContextAttribute);

        if (endpointAttribute is null || contextAttribute is null || semanticTargetInformations.Where(x => x.Target is AutoFastEndpointsAttribute) is null)
        {
            return GenerationConfig.Empty;
        }

        var entityClassDeclarations = semanticTargetInformations
            .Where(x => x.Target is AutoFastEndpointsAttribute && x.ClassDeclarationSyntax is not null)
            .Select(x => x.ClassDeclarationSyntax!)
            .ToImmutableArray();

        if (entityClassDeclarations.Length == 0) return GenerationConfig.Empty;

        var entityConfigs = YieldEntityConfigs(entityClassDeclarations, compilation, ct).ToImmutableArray();

        var semanticTarget = semanticTargetInformations?.FirstOrDefault(x => x.Target == AutoFastContextAttribute);

        if (semanticTarget is null || compilation
                .GetSemanticModel(semanticTarget.ClassDeclarationSyntax!.SyntaxTree)
                .GetDeclaredSymbol(semanticTarget.ClassDeclarationSyntax!) is not INamedTypeSymbol namedTypeSymbol)
        {
            return new GenerationConfig(
                new EntityGenerationConfig(entityConfigs, GetNamespace(entityClassDeclarations.First())));
        }

        return new GenerationConfig(
            new EntityGenerationConfig(entityConfigs, GetNamespace(semanticTarget.ClassDeclarationSyntax)),
            new ContextGenerationConfig(NormaliseName(namedTypeSymbol)));
    }

    private static IEnumerable<EntityConfig> YieldEntityConfigs(ImmutableArray<ClassDeclarationSyntax> entityClassDeclarations, Compilation compilation, CancellationToken ct)
    {
        foreach (var entityClassDeclaration in entityClassDeclarations)
        {
            ct.ThrowIfCancellationRequested();

            var semanticModel = compilation.GetSemanticModel(entityClassDeclaration.SyntaxTree);

            if (semanticModel.GetDeclaredSymbol(entityClassDeclaration) is not INamedTypeSymbol namedTypeSymbol) continue;

            var name = NormaliseName(namedTypeSymbol).Replace("Config", "");

            var entityConfigClassDeclarationSyntaxes = entityClassDeclaration
                .ChildNodes()
                .Where(x => x.Kind() == SyntaxKind.ClassDeclaration)?
                .Cast<ClassDeclarationSyntax>();

            if (entityConfigClassDeclarationSyntaxes is null)
            {
                // throw invalid config
                continue;
            }

            var propertiesClass = entityConfigClassDeclarationSyntaxes.SingleOrDefault(x => x.Identifier.Text is "Properties");

            if (propertiesClass is null)
            {
                // log warning no properties
                yield return new EntityConfig(name);
                continue;
            }

            var members = semanticModel.GetDeclaredSymbol(propertiesClass)!.GetMembers();

            var propertyMetadatas = YieldPropertyMetadatas(members).ToImmutableArray();

            yield return new EntityConfig(name, propertyMetadatas);
        }
    }

    private static string NormaliseName(INamedTypeSymbol namedTypeSymbol)
    {
        var tempName = namedTypeSymbol.ToString().Split('.');
        var name = string.Join(".", tempName.Skip(tempName.Length - 1));
        return name;
    }

    private static IEnumerable<PropertyMetadata> YieldPropertyMetadatas(ImmutableArray<ISymbol> members)
    {
        foreach (var member in members)
        {
            if (member is not IPropertySymbol property) continue;

            var propertyString = property.ToDisplayString(new SymbolDisplayFormat(
                propertyStyle: SymbolDisplayPropertyStyle.ShowReadWriteDescriptor,
                memberOptions: SymbolDisplayMemberOptions.IncludeAccessibility));

            var source = propertyString.Insert(propertyString.IndexOf(' '), $" {property.Type.ToDisplayString()}");

            var attributes = YieldAttributeMetadatas(property).ToImmutableArray();

            yield return new PropertyMetadata(source, property.Name, attributes);

        }
    }

    private static IEnumerable<AttributeMetadata> YieldAttributeMetadatas(IPropertySymbol propertyMember)
    {
        foreach (var attributeData in propertyMember.GetAttributes())
        {
            if (attributeData.AttributeClass is null) continue;

            var attributeName = attributeData.AttributeClass.Name.Split('.').Last().Replace("Attribute", "");

            if (_propertyTargetAttributeNames.Contains(attributeName))
            {
                yield return new(AttributeType.Target, attributeName);
                continue;
            }

            yield return new(AttributeType.Appliable, attributeName);
            continue;
        }
    }

    private static string GetNamespace(ClassDeclarationSyntax enumDeclarationSyntax)
    {
        var nameSpace = string.Empty;

        var potentialNamespaceParent = enumDeclarationSyntax.Parent;

        while (potentialNamespaceParent is not null and not NamespaceDeclarationSyntax and not FileScopedNamespaceDeclarationSyntax)
        {
            potentialNamespaceParent = potentialNamespaceParent.Parent;
        }

        if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
        {
            nameSpace = namespaceParent.Name.ToString();
            while (true)
            {
                if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
                {
                    break;
                }

                namespaceParent = parent;
                nameSpace = $"{namespaceParent.Name}.{nameSpace}";
            }
        }

        return nameSpace;
    }
}
