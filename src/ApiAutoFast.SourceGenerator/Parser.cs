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

    internal static GenerationConfig? GetGenerationConfig(Compilation compilation, ImmutableArray<SemanticTargetInformation> semanticTargetInformations, CancellationToken ct)
    {
        if (semanticTargetInformations.Where(x => x.Target is AutoFastEndpointsAttribute) is null) return GenerationConfig.Empty;

        var entityClassDeclarations = semanticTargetInformations
            .Where(x => x.Target is AutoFastEndpointsAttribute && x.ClassDeclarationSyntax is not null)
            .Select(x => x.ClassDeclarationSyntax!)
            .ToImmutableArray();

        if (entityClassDeclarations.Length == 0) return GenerationConfig.Empty;

        var entityConfigs = YieldEntityConfig(compilation, entityClassDeclarations, ct).ToImmutableArray();

        var semanticTarget = semanticTargetInformations.FirstOrDefault(x => x.Target == AutoFastContextAttribute);

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
        Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax> entityClassDeclarations)
    {
        foreach (var entityClassDeclaration in entityClassDeclarations)
        {
            var semanticModel = compilation.GetSemanticModel(entityClassDeclaration.SyntaxTree);

            if (semanticModel.GetDeclaredSymbol(entityClassDeclaration) is not INamedTypeSymbol namedTypeSymbol) continue;

            var endpointsAttributeArguments = GetAutoFastEndpointsAttributeArguments(namedTypeSymbol);

            yield return new EntityConfigSetup(entityClassDeclaration, semanticModel, endpointsAttributeArguments);
        }
    }

    private static AutoFastEndpointsAttributeArguments GetAutoFastEndpointsAttributeArguments(INamedTypeSymbol namedTypeSymbol)
    {
        var endpointsAttribute = namedTypeSymbol.GetAttributes()[0];

        var entityName = endpointsAttribute.ConstructorArguments[0].IsNull is false
            ? endpointsAttribute.ConstructorArguments[0].Value as string
            : GetLastPart(namedTypeSymbol.Name).Replace("Entity", "");

        var endpointTarget = (EndpointTargetType)endpointsAttribute.ConstructorArguments[1].Value!;

        return new AutoFastEndpointsAttributeArguments(entityName!, endpointTarget);
    }

    private static IEnumerable<EntityConfig> YieldEntityConfig(
        Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax> entityClassDeclarations,
        CancellationToken ct)
    {
        var entityConfigSetups = YieldEntityConfigSetup(compilation, entityClassDeclarations).ToImmutableArray();

        foreach (var entityConfigSetup in entityConfigSetups)
        {
            ct.ThrowIfCancellationRequested();

            var members = entityConfigSetup.SemanticModel
                .GetDeclaredSymbol(entityConfigSetup.ClassDeclarationSyntax)!
                .GetMembers();

            var foreignEntityNames = entityConfigSetups
                .Where(x => x.EndpointsAttributeArguments.EntityName != entityConfigSetup.EndpointsAttributeArguments.EntityName)
                .Select(x => x.EndpointsAttributeArguments.EntityName)
                .ToImmutableArray();

            var propertyMetadatas = YieldPropertyMetadata(compilation, members, foreignEntityNames).ToImmutableArray();

            yield return new EntityConfig(entityConfigSetup.EndpointsAttributeArguments, propertyMetadatas);
        }
    }

    private static IEnumerable<PropertyMetadata> YieldPropertyMetadata(Compilation compilation, ImmutableArray<ISymbol> members, ImmutableArray<string> foreignEntityNames)
    {
        foreach (var member in members)
        {
            if (member is not IPropertySymbol property) continue;

            if (TryGetDomainValueDefinition(compilation, property, foreignEntityNames, out var domainValueDefinition) is false) continue;

            var propertyString = property.ToDisplayString(new SymbolDisplayFormat(
                propertyStyle: SymbolDisplayPropertyStyle.ShowReadWriteDescriptor,
                memberOptions: SymbolDisplayMemberOptions.IncludeAccessibility));

            var firstSpaceIndex = propertyString.IndexOf(' ');
            // note: might consider using domainvalue.entitytype here to skip the whole valueconversion shenanigans
            var entitySource = propertyString.Insert(firstSpaceIndex, $" {domainValueDefinition.TypeName}");

            if (domainValueDefinition.PropertyRelation.RelationalType is RelationalType.ToOne)
            {
                var extendPropertyNameIndex = propertyString.IndexOf('{') - 1;
                propertyString = propertyString.Insert(extendPropertyNameIndex, $"Id ");
            }

            var requestSource = domainValueDefinition.PropertyRelation.RelationalType is RelationalType.ToMany
                ? string.Empty
                : propertyString.Insert(firstSpaceIndex, $" {domainValueDefinition.RequestType}?");

            var commandSource = domainValueDefinition.PropertyRelation.RelationalType is RelationalType.ToMany
                ? string.Empty
                : propertyString.Insert(firstSpaceIndex, $" {domainValueDefinition.RequestType}");

            var attributes = YieldAttributeMetadata(property).ToImmutableArray();

            var requestModelTarget = GetRequestModelTarget(attributes);

            yield return new PropertyMetadata(entitySource, requestSource, commandSource, property.Name, domainValueDefinition, requestModelTarget, attributes);
        }
    }

    private static bool TryGetDomainValueDefinition(
        Compilation compilation,
        IPropertySymbol property,
        ImmutableArray<string> foreignEntityNames,
        out DomainValueDefinition domainValueDefinition)
    {
        // note: check if ToResponse is implemented for response mapping?
        domainValueDefinition = default;

        var domainValueType = compilation.GetTypeByMetadataName(property.Type.ToString());

        var success = domainValueType?.BaseType?.TypeArguments.Length > 0;

        if (success)
        {
            var requestType = domainValueType!.BaseType!.TypeArguments[0];

            var entityType = domainValueType.BaseType.TypeArguments.Length == 2
                ? requestType
                : domainValueType.BaseType.TypeArguments[1];

            var propertyRelation = GetPropertyRelation(property, foreignEntityNames, entityType);

            var responseType = domainValueType.BaseType.TypeArguments.Length == 4
                ? domainValueType.BaseType.TypeArguments[2]
                : requestType;

            domainValueDefinition = new DomainValueDefinition(
                requestType.ToString(),
                entityType.ToString(),
                responseType.ToString(),
                property.Name,
                property.Type.Name,
                propertyRelation);
        }

        return success;
    }

    private static PropertyRelation GetPropertyRelation(IPropertySymbol property, ImmutableArray<string> foreignEntityNames, ITypeSymbol entityType)
    {
        if (entityType.Name.Contains("ICollection") && entityType is INamedTypeSymbol { TypeArguments.Length: > 0 } namedTypeSymbol)
        {
            var entityTypeName = namedTypeSymbol.TypeArguments[0].Name;

            if (foreignEntityNames.Contains(entityTypeName))
            {
                return new PropertyRelation(entityTypeName, property.Name, RelationalType.ToMany);
            }
        }

        if (foreignEntityNames.Contains(entityType.Name))
        {
            return new PropertyRelation(entityType.Name, property.Name, RelationalType.ToOne);
        }

        return PropertyRelation.None;
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

    private static string GetLastPart(string valueToSubstring, char seperator = '.')
    {
        var index = valueToSubstring.LastIndexOf(seperator);

        if (index == -1) return valueToSubstring;

        var lastPart = valueToSubstring.Substring(index, valueToSubstring.Length - index);

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
