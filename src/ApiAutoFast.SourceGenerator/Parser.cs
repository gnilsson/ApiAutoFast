using ApiAutoFast.SourceGenerator.Configuration;
using ApiAutoFast.SourceGenerator.Configuration.Enums;
using ApiAutoFast.SourceGenerator.Descriptive;
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
                if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol) continue;

                var attributeContainingTypeSymbol = attributeSymbol.ContainingType;

                var fullName = attributeContainingTypeSymbol.ToDisplayString();

                if (fullName is AutoFastEndpointsAttribute or AutoFastContextAttribute)
                {
                    return new SemanticTargetInformation(classDeclarationSyntax, fullName);
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

    private static Dictionary<string, List<(DefinedProperty, DomainValueDefinition)>> YieldDefinedValues(Compilation compilation, ImmutableArray<string> entityNames, EntityConfigSetup entity)
    {
        var members = entity.SemanticModel
            .GetDeclaredSymbol(entity.ClassDeclarationSyntax)!
            .GetMembers();

        var values = new Dictionary<string, List<(DefinedProperty, DomainValueDefinition)>>();

        foreach (var member in members)
        {
            if (member is not IPropertySymbol property) continue;

            if (TryGetDomainValueDefinition(compilation, property, entityNames, out var domainValueDefinition) is false) continue;

            var basePropertySource = property.ToDisplayString(new SymbolDisplayFormat(
                propertyStyle: SymbolDisplayPropertyStyle.ShowReadWriteDescriptor,
                memberOptions: SymbolDisplayMemberOptions.IncludeAccessibility));

            var definedProperty = new DefinedProperty(property.Name, basePropertySource);

            if (values.ContainsKey(domainValueDefinition.TypeName))
            {
                values[domainValueDefinition.TypeName].Add((definedProperty, domainValueDefinition));
                continue;
            }

            values.Add(domainValueDefinition.TypeName, new List<(DefinedProperty, DomainValueDefinition)>
            {
                (definedProperty, domainValueDefinition)
            });
        }

        return values;
    }

    private static IEnumerable<PropertyOutput> YieldPropertySetup2(string entityName, DefinedProperty definedProperty, DomainValueDefinition domainValueDefinition)
    {
        var firstSpaceIndex = definedProperty.BaseSource.IndexOf(' ');

        var type = domainValueDefinition.PropertyRelation.Type is RelationalType.None
            ? domainValueDefinition.TypeName
            : domainValueDefinition.EntityType;

        var entitySource = definedProperty.BaseSource.Insert(firstSpaceIndex, $" {type}");
        var requestSource = definedProperty.BaseSource.Insert(firstSpaceIndex, $" {domainValueDefinition.RequestType}?");
        var commandSource = definedProperty.BaseSource.Insert(firstSpaceIndex, $" {domainValueDefinition.RequestType}");

        var attributes = domainValueDefinition.DomainValueType.GetAttributes();

        if (attributes.Length > 0)
        {
            var includeInCommands = attributes
                .FirstOrDefault(x => x.AttributeClass?.Name == TypeText.AttributeText.IncludeInCommand && x.ConstructorArguments.Length > 0)?
                .ConstructorArguments[0]
                .Values
                .Select(x => (x.Value as Type)!.Name)
                .ToImmutableArray();

            if (includeInCommands?.Length > 0)
            {
                foreach (var include in includeInCommands)
                {
                    yield return new PropertyOutput(include, commandSource, PropertyTarget.CreateCommand, definedProperty.Name, domainValueDefinition.RequestType);
                }
            }
        }

        yield return new PropertyOutput(entityName, entitySource, PropertyTarget.Entity, definedProperty.Name, type, domainValueDefinition.PropertyRelation);

        var propertyName = definedProperty.Name;

        if (domainValueDefinition.PropertyRelation.Type is RelationalType.ToOne)
        {
            var idPropertySource = definedProperty.BaseSource.Insert(firstSpaceIndex, $" {TypeText.Identifier}");
            var extendPropertyNameIndex = idPropertySource.IndexOf('{') - 1;
            idPropertySource = idPropertySource.Insert(extendPropertyNameIndex, $"Id");
            propertyName = $"{definedProperty.Name}Id";

            yield return new PropertyOutput(entityName, idPropertySource, PropertyTarget.Entity, propertyName, TypeText.Identifier, domainValueDefinition.PropertyRelation, PropertyKind.Identifier);
        }

        if (domainValueDefinition.PropertyRelation.Type is not RelationalType.ToMany)
        {
            yield return new PropertyOutput(entityName, requestSource, PropertyTarget.QueryRequest, propertyName, $"{domainValueDefinition.RequestType}?");
            // todo: use flags
            yield return new PropertyOutput(entityName, commandSource, PropertyTarget.CreateCommand, propertyName, domainValueDefinition.RequestType);
            yield return new PropertyOutput(entityName, commandSource, PropertyTarget.ModifyCommand, propertyName, domainValueDefinition.RequestType);
        }
    }

    private static IEnumerable<KeyValuePair<string, Dictionary<string, List<(DefinedProperty, DomainValueDefinition)>>>> YieldPropertyConfigSetup(
        Compilation compilation,
        ImmutableArray<EntityConfigSetup> entityConfigSetups)
    {
        var entityNames = entityConfigSetups
            .Select(x => x.EndpointsAttributeArguments.EntityName)
            .ToImmutableArray();

        foreach (var entity in entityConfigSetups)
        {
            yield return new KeyValuePair<string, Dictionary<string, List<(DefinedProperty, DomainValueDefinition)>>>(
                entity.EndpointsAttributeArguments.EntityName,
                YieldDefinedValues(compilation, entityNames, entity));
        }
    }

    private static IEnumerable<EntityConfig> YieldEntityConfig(
        Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax> entityClassDeclarations,
        CancellationToken ct)
    {
        var entityConfigSetups = YieldEntityConfigSetup(compilation, entityClassDeclarations).ToImmutableArray();

        var propertyConfigSetups = YieldPropertyConfigSetup(compilation, entityConfigSetups).ToImmutableArray();

        foreach (var propertyConfig in YieldPropertyConfig(propertyConfigSetups))
        {
            ct.ThrowIfCancellationRequested();

            var relationalNavigationNames = propertyConfig
                .Properties[PropertyTarget.Entity]
                .Where(x => x.Relation.Type is not RelationalType.None && x.PropertyKind is not PropertyKind.Identifier)
                .Select(x => x.Relation.ForeigEntityProperty)
                .ToImmutableArray();

            var endpointsAttributeArguments = entityConfigSetups
                .First(x => x.EndpointsAttributeArguments.EntityName == propertyConfig.EntityName)
                .EndpointsAttributeArguments;

            yield return new EntityConfig(endpointsAttributeArguments, relationalNavigationNames, propertyConfig);
        }
    }

    private static IEnumerable<PropertyConfig> YieldPropertyConfig(ImmutableArray<KeyValuePair<string, Dictionary<string, List<(DefinedProperty, DomainValueDefinition)>>>> entityNameKeyKvps)
    {
        var propertiesCollection = new Dictionary<string, Dictionary<PropertyTarget, List<PropertyOutput>>>();
        var domainValuesCollection = new Dictionary<string, ImmutableArray<DefinedDomainValue>>();

        foreach (var kvpDictionaries in entityNameKeyKvps)
        {
            var definedDomainValues = new List<DefinedDomainValue>();

            foreach (var kvpProperties in kvpDictionaries.Value)
            {
                var backingIdentifierPropertyNames = new List<string>();

                foreach (var (definedProperty, definedDomainValue) in kvpProperties.Value)
                {
                    foreach (var propertyOutput in YieldPropertySetup2(kvpDictionaries.Key, definedProperty, definedDomainValue))
                    {
                        if (propertyOutput.PropertyKind is PropertyKind.Identifier && propertyOutput.Name == $"{definedProperty.Name}Id")
                        {
                            backingIdentifierPropertyNames.Add(propertyOutput.Name);
                        }

                        if (propertiesCollection.TryGetValue(propertyOutput.EntityKind, out var propertyDictionary) is false)
                        {
                            propertiesCollection.Add(propertyOutput.EntityKind, new Dictionary<PropertyTarget, List<PropertyOutput>>
                            {
                                [propertyOutput.Target] = new List<PropertyOutput> { propertyOutput }
                            });
                            continue;
                        }

                        if (propertyDictionary.TryGetValue(propertyOutput.Target, out var outputs) is false)
                        {
                            propertyDictionary.Add(propertyOutput.Target, new List<PropertyOutput> { propertyOutput });
                            continue;
                        }

                        outputs.Add(propertyOutput);
                    }
                }

                var domainValueDefinition = kvpDictionaries.Value[kvpProperties.Key].First().Item2;
                var definedProperties = kvpProperties.Value.Select(x => x.Item1).ToImmutableArray();
                definedDomainValues.Add(new DefinedDomainValue(domainValueDefinition, definedProperties, backingIdentifierPropertyNames.ToImmutableArray()));
            }

            domainValuesCollection.Add(kvpDictionaries.Key, definedDomainValues.ToImmutableArray());
        }

        foreach (var kvpCollection in propertiesCollection)
        {
            var domainValues = domainValuesCollection[kvpCollection.Key];

            var properties = kvpCollection.Value
                .Select(x => (x.Key, Value: x.Value.ToImmutableArray()))
                .ToImmutableDictionary(x => x.Key, x => x.Value);

            yield return new PropertyConfig(kvpCollection.Key, properties, domainValues);
        }
    }

    private static bool TryGetDomainValueDefinition(
        Compilation compilation,
        IPropertySymbol property,
        ImmutableArray<string> entityNames,
        out DomainValueDefinition domainValueDefinition)
    {
        domainValueDefinition = default;

        var domainValueType = compilation.GetTypeByMetadataName(property.Type.ToString());

        var success = domainValueType?.BaseType?.TypeArguments.Length > 0;

        if (success)
        {
            var requestType = domainValueType!.BaseType!.TypeArguments[0];

            var entityType = domainValueType.BaseType.TypeArguments.Length == 2
                ? requestType
                : domainValueType.BaseType.TypeArguments[1];

            var responseType = domainValueType.BaseType.TypeArguments.Length == 4
                ? domainValueType.BaseType.TypeArguments[2]
                : requestType;

            var propertyRelation = GetPropertyRelation(property, entityNames, entityType);

            domainValueDefinition = new DomainValueDefinition(
                domainValueType,
                requestType.ToString(),
                entityType.ToString(),
                responseType.ToString(),
                property.Name,
                property.Type.Name,
                propertyRelation);
        }

        return success;
    }

    private static PropertyRelation GetPropertyRelation(IPropertySymbol property, ImmutableArray<string> entityNames, ITypeSymbol entityType)
    {
        if (entityType.Name.Contains(TypeText.ICollection) && entityType is INamedTypeSymbol { TypeArguments.Length: > 0 } namedTypeSymbol)
        {
            var entityTypeName = namedTypeSymbol.TypeArguments[0].Name;

            if (entityNames.Contains(entityTypeName))
            {
                return new PropertyRelation(entityTypeName, property.Name, RelationalType.ToMany);
            }
        }

        if (entityNames.Contains(entityType.Name))
        {
            return new PropertyRelation(entityType.Name, property.Name, RelationalType.ToOne);
        }

        return PropertyRelation.None;
    }

    private static string GetLastPart(string valueToSubstring, char seperator = '.')
    {
        var index = valueToSubstring.LastIndexOf(seperator);

        if (index == -1) return valueToSubstring;

        var lastPart = valueToSubstring.Substring(index, valueToSubstring.Length - index);

        return lastPart;
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

//private static IEnumerable<PropertyAttributeMetadata> YieldAttributeMetadata(IPropertySymbol propertyMember)
//{
//    foreach (var attributeData in propertyMember.GetAttributes())
//    {
//        if (attributeData.AttributeClass is null) continue;

//        var attributeName = GetLastPart(attributeData.AttributeClass.Name).Replace(TypeText.Attribute, "");

//        if (attributeName == TypeText.AttributeText.ExcludeRequestModel)
//        {
//            if (attributeData.ConstructorArguments.Length > 0)
//            {
//                yield return new PropertyAttributeMetadata(
//                    AttributeType.Custom,
//                    attributeName,
//                    (RequestModelTarget)attributeData.ConstructorArguments[0].Value!);

//                continue;
//            }

//            yield return new PropertyAttributeMetadata(AttributeType.Custom, attributeName, RequestModelTarget.None);
//            continue;
//        }

//        yield return new PropertyAttributeMetadata(AttributeType.Default, attributeName);
//    }
//}

//private static RequestModelTarget GetRequestModelTarget(ImmutableArray<PropertyAttributeMetadata> attributes)
//{
//    if (attributes.Length > 0)
//    {
//        var attriubteMetadata = attributes.FirstOrDefault(x => x.RequestModelTarget is not null);

//        if (attriubteMetadata.RequestModelTarget.HasValue)
//        {
//            return attriubteMetadata.RequestModelTarget.Value;
//        }
//    }

//    return RequestModelTarget.CreateCommand | RequestModelTarget.ModifyCommand | RequestModelTarget.QueryRequest;
//}
