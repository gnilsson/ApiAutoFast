using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using ApiAutoFast.SourceGenerator.Configuration;

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

        var entityConfigs = YieldEntityConfig(entityClassDeclarations, compilation, ct).ToImmutableArray();

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

    private static IEnumerable<EntityConfigSetup> YieldEntityConfigSetup(
        ImmutableArray<ClassDeclarationSyntax> entityClassDeclarations,
        Compilation compilation)
    {
        foreach (var entityClassDeclaration in entityClassDeclarations)
        {
            var semanticModel = compilation.GetSemanticModel(entityClassDeclaration.SyntaxTree);

            if (semanticModel.GetDeclaredSymbol(entityClassDeclaration) is not INamedTypeSymbol namedTypeSymbol) continue;

            var name = GetEntityName(namedTypeSymbol);

            yield return new EntityConfigSetup(entityClassDeclaration, semanticModel, name);
        }
    }

    private static IEnumerable<EntityConfig> YieldEntityConfig(
        ImmutableArray<ClassDeclarationSyntax> entityClassDeclarations,
        Compilation compilation,
        CancellationToken ct)
    {
        var entityConfigSetups = YieldEntityConfigSetup(entityClassDeclarations, compilation).ToImmutableArray();

        foreach (var entityConfigSetup in entityConfigSetups)
        {
            ct.ThrowIfCancellationRequested();

            var entitySubClassDeclarations = entityConfigSetup.ClassDeclarationSyntax
                .ChildNodes()
                .Where(x => x.Kind() is SyntaxKind.ClassDeclaration)
                .Select(x => x as ClassDeclarationSyntax)
                .ToImmutableArray();

            if (entitySubClassDeclarations.Length <= 0
                || entitySubClassDeclarations.SingleOrDefault(x => x!.Identifier.Text is "Properties") is not ClassDeclarationSyntax propertiesClass)
            {
                // todo: throw invalid config
                continue;
            }

            var members = entityConfigSetup.SemanticModel.GetDeclaredSymbol(propertiesClass)!.GetMembers();

            var foreignEntityNames = entityConfigSetups
                .Where(x => x.Name != entityConfigSetup.Name)
                .Select(x => x.Name)
                .ToImmutableArray();

            var propertyMetadatas = YieldPropertyMetadata(members, foreignEntityNames).ToImmutableArray();

            yield return new EntityConfig(entityConfigSetup.Name, propertyMetadatas);
        }
    }

    private static IEnumerable<PropertyMetadata> YieldPropertyMetadata(ImmutableArray<ISymbol> members, ImmutableArray<string> foreignEntityNames)
    {
        foreach (var member in members)
        {
            if (member is not IPropertySymbol property) continue;

            var propertyString = property.ToDisplayString(new SymbolDisplayFormat(
                propertyStyle: SymbolDisplayPropertyStyle.ShowReadWriteDescriptor,
                memberOptions: SymbolDisplayMemberOptions.IncludeAccessibility));

            var relational = GetRelationalEntity(foreignEntityNames, property);

            var source = GetPropertySource(property, propertyString, relational);

            var attributes = YieldAttributeMetadatas(property).ToImmutableArray();

            yield return new PropertyMetadata(source, property.Name, attributes, relational);
        }
    }

    private static PropertySource GetPropertySource(IPropertySymbol property, string propertyString, PropertyRelational? relational)
    {
        // note: define type seperately across requests and entity
        var type = relational switch
        {
            null or
            { RelationalType: RelationalType.ToManyHidden or RelationalType.ToOneHidden } => property.Type.ToDisplayString(),
            { RelationalType: RelationalType.ToMany } => $"ICollection<{relational.Value.ForeignEntityName}>",
            { RelationalType: RelationalType.ToOne } => relational.Value.ForeignEntityName,
            _ => "object"
        };

        var entitySource = propertyString.Insert(propertyString.IndexOf(' '), $" {type}");

        // note: temporary way of checking types
        //       nullable?
        if (type.Contains("Identifier"))
        {
            if (type.Contains("ICollection"))
            {
                return new PropertySource(entitySource, propertyString.Insert(propertyString.IndexOf(' '), $" IEnumerable<string>?"));
            }

            return new PropertySource(entitySource, propertyString.Insert(propertyString.IndexOf(' '), $" string?"));
        }

        return new PropertySource(entitySource);
    }

    // note: this method is based on some conventions, i.e that an entity with a one-to-many relation will declare that
    //       with a property of type ICollection<Foo> and with name Foos.
    private static PropertyRelational? GetRelationalEntity(ImmutableArray<string> foreignEntityNames, IPropertySymbol property)
    {
        foreach (var foreignEntityName in foreignEntityNames)
        {
            if (property.Name.Contains(foreignEntityName) is false) continue;

            if (property.Type.ToDisplayString().Contains("ICollection"))
            {
                if ($"{foreignEntityName}s" == property.Name)
                {
                    return new PropertyRelational(foreignEntityName, property.Name, RelationalType.ToMany);
                }

                return new PropertyRelational(foreignEntityName, property.Name, RelationalType.ToManyHidden);
            }

            if (foreignEntityName == property.Name)
            {
                return new PropertyRelational(foreignEntityName, property.Name, RelationalType.ToOne);
            }

            return new PropertyRelational(foreignEntityName, property.Name, RelationalType.ToOneHidden);
        }

        return null;
    }

    private static string GetEntityName(INamedTypeSymbol namedTypeSymbol)
    {
        var endpointsAttribute = namedTypeSymbol.GetAttributes()[0];

        if (endpointsAttribute.ConstructorArguments.Length > 0 && endpointsAttribute.ConstructorArguments[0].IsNull is false)
        {
            return (endpointsAttribute.ConstructorArguments[0].Value as string)!;
        }

        return NormaliseName(namedTypeSymbol).Replace("Config", "");
    }

    private static string NormaliseName(INamedTypeSymbol namedTypeSymbol)
    {
        var tempName = namedTypeSymbol.ToString().Split('.');
        var name = string.Join(".", tempName.Skip(tempName.Length - 1));
        return name;
    }

    private static IEnumerable<PropertyAttributeMetadata> YieldAttributeMetadatas(IPropertySymbol propertyMember)
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
