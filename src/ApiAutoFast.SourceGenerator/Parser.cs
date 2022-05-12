using ApiAutoFast.SourceGenerator.Configuration;
using ApiAutoFast.SourceGenerator.Configuration.Enums;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace ApiAutoFast.SourceGenerator;

internal static class Parser
{
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
            new ContextGenerationConfig(namedTypeSymbol.Name));
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

            // note: this configuration system should basically be reworked
            if (entitySubClassDeclarations.Length <= 0
                || entitySubClassDeclarations.SingleOrDefault(x => x!.Identifier.Text is "Properties") is not ClassDeclarationSyntax propertiesClass)
            {
                continue;
            }

            var members = entityConfigSetup.SemanticModel.GetDeclaredSymbol(propertiesClass)!.GetMembers();

            var foreignEntityNames = entityConfigSetups
                .Where(x => x.Name != entityConfigSetup.Name)
                .Select(x => x.Name)
                .ToImmutableArray();

            var propertyMetadatas = YieldPropertyMetadata(members, foreignEntityNames, entityConfigSetup.SemanticModel).ToImmutableArray();

            yield return new EntityConfig(entityConfigSetup.Name, propertyMetadatas);
        }
    }

    private static IEnumerable<PropertyMetadata> YieldPropertyMetadata(ImmutableArray<ISymbol> members, ImmutableArray<string> foreignEntityNames, SemanticModel semanticModel)
    {
        // todo: stop generation if any property is property.type.typekind error
        foreach (var member in members)
        {
            if (member is not IPropertySymbol property) continue;

            var propertyString = property.ToDisplayString(new SymbolDisplayFormat(
                propertyStyle: SymbolDisplayPropertyStyle.ShowReadWriteDescriptor,
                memberOptions: SymbolDisplayMemberOptions.IncludeAccessibility));

            var typeKind = GetTypeKind(property);

            var relational = GetRelationalEntity(foreignEntityNames, property);

            var source = GetPropertySource(property, propertyString, relational);

            var attributes = YieldAttributeMetadata(property).ToImmutableArray();

            var requestModelTarget = GetRequestModelTarget(attributes);

            yield return new PropertyMetadata(source, property.Name, typeKind == "Enum", requestModelTarget, attributes, relational);
        }
    }

    private static RequestModelTarget GetRequestModelTarget(ImmutableArray<PropertyAttributeMetadata> attributes)
    {
        if (attributes.Length > 0)
        {
            var attriubteMetadata = attributes.FirstOrDefault(x => x.RequestModelTarget is not null);

            if (attriubteMetadata.RequestModelTarget.HasValue)
            {
                return attriubteMetadata.RequestModelTarget.Value;
            }
        }

        return RequestModelTarget.CreateCommand | RequestModelTarget.ModifyCommand | RequestModelTarget.QueryRequest;
    }

    private static string GetTypeKind(IPropertySymbol property)
    {
        if (property.NullableAnnotation is NullableAnnotation.Annotated)
        {
            if (property.Type is INamedTypeSymbol { TypeArguments.Length: > 0 } namedTypeSymbol)
            {
                return namedTypeSymbol.TypeArguments[0].TypeKind.ToString();
            }
        }

        return property.Type.TypeKind.ToString();
    }

    private static PropertySource GetPropertySource(IPropertySymbol property, string propertyString, PropertyRelational? relational)
    {
        // note: define type seperately across requests and entity
        var type = relational switch
        {
            null or
            { RelationalType: RelationalType.ShadowToMany or RelationalType.ShadowToOne } => property.Type.ToDisplayString(),
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
    //       alot of this is temporary.
    private static PropertyRelational? GetRelationalEntity(ImmutableArray<string> foreignEntityNames, IPropertySymbol property)
    {
        foreach (var foreignEntityName in foreignEntityNames)
        {
            if (property.Name.Contains(foreignEntityName) is false) continue;

            if (property.Type.ToDisplayString().Contains("ICollection"))
            {
                if (property.Name == $"{foreignEntityName}s")
                {
                    return new PropertyRelational(foreignEntityName, property.Name, RelationalType.ToMany);
                }

                if (property.Name == $"{foreignEntityName}Ids")
                {
                    return new PropertyRelational(foreignEntityName, property.Name, RelationalType.ShadowToMany);
                }

                // note: we need exact match to be certain, so just continue here.
                continue;
            }

            if (property.Name == foreignEntityName)
            {
                return new PropertyRelational(foreignEntityName, property.Name, RelationalType.ToOne);
            }

            if (property.Name == $"{foreignEntityName}Id")
            {
                return new PropertyRelational(foreignEntityName, property.Name, RelationalType.ShadowToOne);
            }
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

        // note: getlastpart here might be nessescary in case of full name qualification
        return GetLastPart(namedTypeSymbol.Name).Replace("Config", "");
    }

    private static string GetLastPart(string @string, char seperator = '.')
    {
        var index = @string.LastIndexOf(seperator);

        if (index == -1) return @string;

        var lastPart = @string.Substring(index, @string.Length - index);

        return lastPart;
    }

    private static IEnumerable<PropertyAttributeMetadata> YieldAttributeMetadata(IPropertySymbol propertyMember)
    {
        foreach (var attributeData in propertyMember.GetAttributes())
        {
            if (attributeData.AttributeClass is null) continue;

            var attributeName = GetLastPart(attributeData.AttributeClass.Name).Replace("Attribute", "");

            if (attributeName == "ExcludeRequestModel")
            {
                if (attributeData.ConstructorArguments.Length > 0)
                {
                    yield return new PropertyAttributeMetadata(
                        AttributeType.Custom,
                        attributeName,
                        (RequestModelTarget)attributeData.ConstructorArguments[0].Value!);

                    continue;
                }

                yield return new PropertyAttributeMetadata(AttributeType.Custom, attributeName, RequestModelTarget.None);
                continue;
            }

            yield return new PropertyAttributeMetadata(AttributeType.Default, attributeName);
        }
    }

    private static string GetNamespace(ClassDeclarationSyntax classDeclarationSyntax)
    {
        var nameSpace = string.Empty;

        var potentialNamespaceParent = classDeclarationSyntax.Parent;

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
