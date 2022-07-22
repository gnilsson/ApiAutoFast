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
    private const string AutoFastEntityAttribute = "ApiAutoFast.AutoFastEntityAttribute";
    private const string AutoFastContextAttribute = "ApiAutoFast.AutoFastContextAttribute";
    private const string AutoFastEndpointAttribute = "ApiAutoFast.AutoFastEndpointAttribute";

    internal static SemanticTargetInformation? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var classDeclarationSyntax = context.Node as ClassDeclarationSyntax;

        foreach (var attributeListSyntax in classDeclarationSyntax!.AttributeLists)
        {
            foreach (var attributeSyntax in attributeListSyntax.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol) continue;

                var fullName = attributeSymbol.ContainingType.ToDisplayString();

                if (fullName is AutoFastEntityAttribute or AutoFastContextAttribute or AutoFastEndpointAttribute)
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
        if (semanticTargetInformations.Any(x => x.Target is AutoFastEntityAttribute) is false) return null;

        ct.ThrowIfCancellationRequested();

        var entityClassDeclarations = semanticTargetInformations
            .Where(x => x.Target is AutoFastEntityAttribute && x.ClassDeclarationSyntax is not null)
            .Select(x => x.ClassDeclarationSyntax)
            .ToImmutableArray();

        if (entityClassDeclarations.Length is 0) return null;

        var entityConfigSetups = YieldEntityConfigSetup(compilation, entityClassDeclarations).ToImmutableArray();

        var entities = entityConfigSetups.ToImmutableDictionary(x => x.EndpointsAttributeArguments.EntityName, y => y.EndpointsAttributeArguments.IdType);

        var endpointClassDeclarations = semanticTargetInformations
            .Where(x => x.Target is AutoFastEndpointAttribute && x.ClassDeclarationSyntax is not null)
            .Select(x => x.ClassDeclarationSyntax)
            .ToImmutableArray();

        var targetedEndpointNames = endpointClassDeclarations.Length is 0
            ? null
            : YieldTargetedEndpointName(compilation, endpointClassDeclarations, entities.Keys)?.ToImmutableArray();

        var propertyConfigSetups = YieldPropertyConfigSetup(compilation, entities.Keys, entityConfigSetups).ToImmutableArray();

        var entityConfigs = YieldEntityConfig(entityConfigSetups, propertyConfigSetups, entities, ct).ToImmutableArray();

        var semanticTarget = semanticTargetInformations.FirstOrDefault(x => x.Target is AutoFastContextAttribute);

        if (semanticTarget is null || compilation
            .GetSemanticModel(semanticTarget.ClassDeclarationSyntax.SyntaxTree)
            .GetDeclaredSymbol(semanticTarget.ClassDeclarationSyntax) is not INamedTypeSymbol namedTypeSymbol)
        {
            return new GenerationConfig(
                new EntityGenerationConfig(entityConfigs, GetNamespace(entityClassDeclarations.First()), targetedEndpointNames));
        }

        return new GenerationConfig(
            new EntityGenerationConfig(entityConfigs, GetNamespace(semanticTarget.ClassDeclarationSyntax), targetedEndpointNames),
            new ContextGenerationConfig(namedTypeSymbol.Name));
    }

    private static IEnumerable<string> YieldTargetedEndpointName(
        Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax> endpointClassDeclarations,
        IEnumerable<string> foreignEntityNames)
    {
        var endpoints = foreignEntityNames
            .Select(x => $"{x}Endpoint")
            .ToArray();

        foreach (var classDeclaration in endpointClassDeclarations)
        {
            var declaredSymbol = compilation
                .GetSemanticModel(classDeclaration.SyntaxTree)
                .GetDeclaredSymbol(classDeclaration);

            if (declaredSymbol is null) continue;

            if (declaredSymbol.Name.EndsWith("Extended"))
            {
                yield return declaredSymbol.Name.Replace("Extended", "");
                continue;
            }

            var endpointTarget = declaredSymbol?.BaseType?.Name;

            if (endpointTarget is not null && endpoints.Any(x => endpointTarget.Contains(x)))
            {
                yield return endpointTarget;
            }
        }
    }

    private static IEnumerable<EntityConfig> YieldEntityConfig(
        ImmutableArray<EntityConfigSetup> entityConfigSetups,
        ImmutableArray<KeyValuePair<string, Dictionary<string, List<PropertySetup>>>> propertyConfigSetups,
        ImmutableDictionary<string, string> foreignEntities,
        CancellationToken ct)
    {
        foreach (var propertyConfig in YieldPropertyConfig(propertyConfigSetups, foreignEntities))
        {
            ct.ThrowIfCancellationRequested();

            var relationalNavigationNames = propertyConfig
                .Properties
                .Where(x => x.Target is PropertyTarget.Entity && x.Relation.Type is not RelationalType.None && x.PropertyKind is not PropertyKind.Identifier)
                .Select(x => x.Relation.ForeigEntityProperty)
                .ToImmutableArray();

            var endpointsAttributeArguments = entityConfigSetups
                .First(x => x.EndpointsAttributeArguments.EntityName == propertyConfig.EntityName)
                .EndpointsAttributeArguments;

            var stringEntityProperties = propertyConfig.DomainValues
                .Where(x => x.DomainValueDefinition.EntityType is TypeText.String)
                .SelectMany(x => x.DefinedProperties.Select(x => x.Name))
                .ToImmutableArray();

            yield return new EntityConfig(endpointsAttributeArguments, propertyConfig, relationalNavigationNames, stringEntityProperties);
        }
    }

    private static IEnumerable<EntityConfigSetup> YieldEntityConfigSetup(
        Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax> entityClassDeclarations)
    {
        foreach (var entityClassDeclaration in entityClassDeclarations)
        {
            var semanticModel = compilation.GetSemanticModel(entityClassDeclaration.SyntaxTree);

            if (semanticModel.GetDeclaredSymbol(entityClassDeclaration) is not INamedTypeSymbol namedTypeSymbol) continue;

            var endpointsAttributeArguments = GetAutoFastEntityAttributeArguments(namedTypeSymbol);

            yield return new EntityConfigSetup(entityClassDeclaration, semanticModel, endpointsAttributeArguments);
        }
    }

    private static EntityAttributeArguments GetAutoFastEntityAttributeArguments(INamedTypeSymbol namedTypeSymbol)
    {
        var endpointsAttribute = namedTypeSymbol.GetAttributes()[0];

        var entityName = endpointsAttribute.ConstructorArguments[0].IsNull is false
            ? endpointsAttribute.ConstructorArguments[0].Value as string
            : GetLastPart(namedTypeSymbol.Name).Replace("Entity", "");

        var endpointTarget = (EndpointTargetType)endpointsAttribute.ConstructorArguments[1].Value!;

        var idType = ((IdType)endpointsAttribute.ConstructorArguments[2].Value!).ToString();

        return new EntityAttributeArguments(entityName!, endpointTarget, idType);
    }

    private static IEnumerable<KeyValuePair<string, Dictionary<string, List<PropertySetup>>>> YieldPropertyConfigSetup(
        Compilation compilation,
        IEnumerable<string> entityNames,
        ImmutableArray<EntityConfigSetup> entityConfigSetups)
    {
        foreach (var entity in entityConfigSetups)
        {
            var members = entity.SemanticModel
                .GetDeclaredSymbol(entity.ClassDeclarationSyntax)!
                .GetMembers();

            yield return new KeyValuePair<string, Dictionary<string, List<PropertySetup>>>(
                entity.EndpointsAttributeArguments.EntityName,
                GetPropertySetups(compilation, entityNames.ToImmutableArray(), members));
        }
    }

    private static Dictionary<string, List<PropertySetup>> GetPropertySetups(Compilation compilation, ImmutableArray<string> entityNames, ImmutableArray<ISymbol> members)
    {
        var propertySetupDictionary = new Dictionary<string, List<PropertySetup>>();

        foreach (var member in members)
        {
            if (member is not IPropertySymbol property) continue;

            if (TryGetDomainValueMetadata(compilation, property, entityNames, out var domainValueMetadata) is false) continue;

            var basePropertySource = property.ToDisplayString(new SymbolDisplayFormat(
                propertyStyle: SymbolDisplayPropertyStyle.ShowReadWriteDescriptor,
                memberOptions: SymbolDisplayMemberOptions.IncludeAccessibility));

            var propertySetup = new PropertySetup(property.Name, basePropertySource, domainValueMetadata);

            if (propertySetupDictionary.ContainsKey(domainValueMetadata.Definition.TypeName))
            {
                propertySetupDictionary[domainValueMetadata.Definition.TypeName].Add(propertySetup);
                continue;
            }

            propertySetupDictionary.Add(domainValueMetadata.Definition.TypeName, new List<PropertySetup> { propertySetup });
        }

        return propertySetupDictionary;
    }

    private static bool TryGetDomainValueMetadata(
        Compilation compilation,
        IPropertySymbol property,
        ImmutableArray<string> entityNames,
        out DomainValueMetadata domainValueMetadata)
    {
        domainValueMetadata = default;

        var domainValueType = compilation.GetTypeByMetadataName(property.Type.ToString());

        if (domainValueType is null) return false;

        var domainValueDefinition = GetDomainValueDefinition(domainValueType, property, entityNames);

        if (domainValueDefinition.HasValue is false) return false;

        var attributes = domainValueType.GetAttributes();

        domainValueMetadata = new DomainValueMetadata(domainValueDefinition.Value, attributes);

        return true;
    }

    private static DomainValueDefinition? GetDomainValueDefinition(
        INamedTypeSymbol domainValueType,
        IPropertySymbol property,
        ImmutableArray<string> entityNames)
    {
        // note: attempt for default domain values.. for great benefit?
        if (domainValueType.BaseType?.Name is TypeText.StringDomainValue)
        {
            return new DomainValueDefinition(
                TypeText.String,
                TypeText.String,
                TypeText.String,
                property.Type.Name,
                PropertyRelation.None);
        }

        if (domainValueType.BaseType?.TypeArguments.Length > 0)
        {
            var requestType = domainValueType!.BaseType!.TypeArguments[0];

            var entityType = domainValueType.BaseType.TypeArguments.Length == 2
                ? requestType
                : domainValueType.BaseType.TypeArguments[1];

            var responseType = domainValueType.BaseType.TypeArguments.Length == 4
                ? domainValueType.BaseType.TypeArguments[2]
                : requestType;

            var propertyRelation = GetPropertyRelation(property, entityNames, entityType);

            return new DomainValueDefinition(
                requestType.ToString(),
                entityType.ToString(),
                responseType.ToString(),
                property.Type.Name,
                propertyRelation);
        }

        return null;
    }

    private static IEnumerable<PropertyConfig> YieldPropertyConfig(
        ImmutableArray<KeyValuePair<string, Dictionary<string, List<PropertySetup>>>> propertyConfigSetupsKvps,
        ImmutableDictionary<string, string> entities)
    {
        var propertiesCollection = new Dictionary<string, List<PropertyOutput>>();
        var domainValuesCollection = new Dictionary<string, ImmutableArray<DefinedDomainValue>>();
        foreach (var propertyConfigSetupsEntityKvp in propertyConfigSetupsKvps)
        {
            var definedDomainValues = ImmutableArray.CreateBuilder<DefinedDomainValue>();
            foreach (var propertyConfigSetupsKvp in propertyConfigSetupsEntityKvp.Value)
            {
                var domainValueDefinition = propertyConfigSetupsEntityKvp.Value[propertyConfigSetupsKvp.Key].First().DomainValueMetadata.Definition;

                var definedProperties = ImmutableArray.CreateBuilder<DefinedProperty>();
                foreach (var propertySetup in propertyConfigSetupsKvp.Value)
                {
                    foreach (var propertyOutput in YieldPropertyOutput(propertyConfigSetupsEntityKvp.Key, propertySetup, entities))
                    {
                        if (propertyOutput.PropertyKind is PropertyKind.Identifier && propertyOutput.Name == $"{propertySetup.Name}Id")
                        {
                            definedProperties.Add(new DefinedProperty(propertyOutput.Name, PropertyKind.Identifier, entities[domainValueDefinition.PropertyRelation.ForeignEntityName]));
                        }

                        if (propertiesCollection.TryGetValue(propertyOutput.EntityKind, out var properties))
                        {
                            properties.Add(propertyOutput);
                            continue;
                        }

                        propertiesCollection.Add(propertyOutput.EntityKind, new List<PropertyOutput> { propertyOutput });
                    }

                    definedProperties.Add(new DefinedProperty(propertySetup.Name, PropertyKind.Domain, domainValueDefinition.TypeName));
                }

                definedDomainValues.Add(new DefinedDomainValue(domainValueDefinition, definedProperties.ToImmutable()));
            }

            domainValuesCollection.Add(propertyConfigSetupsEntityKvp.Key, definedDomainValues.ToImmutable());
        }

        foreach (var kvpCollection in propertiesCollection)
        {
            var domainValues = domainValuesCollection[kvpCollection.Key];

            var properties = kvpCollection.Value.ToImmutableArray();

            yield return new PropertyConfig(kvpCollection.Key, properties, domainValues);
        }
    }

    private static IEnumerable<PropertyOutput> YieldPropertyOutput(string entityName, PropertySetup propertySetup, ImmutableDictionary<string, string> entityIdTypes)
    {
        var domainValueDefinition = propertySetup.DomainValueMetadata.Definition;

        var propertySource = GetPropertySource(propertySetup, domainValueDefinition, entityIdTypes);

        foreach (var propertyOutput in YieldIncludedEntityOutput(propertySetup, propertySource))
        {
            yield return propertyOutput;
        }

        yield return new PropertyOutput(entityName, propertySource.Entity!, PropertyTarget.Entity, propertySetup.Name, domainValueDefinition.PropertyRelation);
        yield return new PropertyOutput(entityName, propertySource.Response!, PropertyTarget.Response, propertySetup.Name, domainValueDefinition.PropertyRelation);

        if (domainValueDefinition.PropertyRelation.Type is RelationalType.ToOne)
        {
            yield return new PropertyOutput(entityName, propertySource.Id!, PropertyTarget.Entity, $"{propertySetup.Name}Id", domainValueDefinition.PropertyRelation, PropertyKind.Identifier);
            yield return new PropertyOutput(entityName, propertySource.ResponseId!, PropertyTarget.Response, propertySetup.Name, domainValueDefinition.PropertyRelation, PropertyKind.Identifier);
        }

        //if (domainValueDefinition.PropertyRelation.Type is not RelationalType.ToMany)
        //{
        //    yield return new PropertyOutput(entityName, propertySource.Request!, PropertyTarget.QueryRequest, propertySetup.Name);
        //    yield break;
        //}

        yield return new PropertyOutput(entityName, propertySource.Request!, PropertyTarget.CreateCommand | PropertyTarget.ModifyCommand | PropertyTarget.QueryRequest, propertySetup.Name, domainValueDefinition.PropertyRelation);

        if (domainValueDefinition.PropertyRelation.Type is RelationalType.ToMany)
        {
            yield return new PropertyOutput(entityName, propertySource.ResponseId!, PropertyTarget.Response, propertySetup.Name, domainValueDefinition.PropertyRelation, PropertyKind.Identifier);
        }
    }

    private static IEnumerable<PropertyOutput> YieldIncludedEntityOutput(PropertySetup propertySetup, PropertySource propertySource)
    {
        if (propertySetup.DomainValueMetadata.AttributeDatas.Length > 0)
        {
            var entityName = propertySetup.DomainValueMetadata.AttributeDatas
                .FirstOrDefault(x => x.AttributeClass?.Name is TypeText.IncludeInCommandAttribute && x.ConstructorArguments.Length > 0)?.ConstructorArguments[0];

            if (entityName is null) yield break;

            if (entityName.Value.Kind is TypedConstantKind.Array)
            {
                foreach (var name in entityName.Value.Values)
                {
                    yield return new PropertyOutput((name.Value as INamedTypeSymbol)!.Name, propertySource.Command!, PropertyTarget.CreateCommand, propertySetup.Name);
                }
                yield break;
            }

            yield return new PropertyOutput((entityName.Value.Value as INamedTypeSymbol)!.Name, propertySource.Command!, PropertyTarget.CreateCommand, propertySetup.Name);
        }
    }

    // dictionary.. [key string] { source, type }

    private static PropertySource GetPropertySource(PropertySetup propertySetup, DomainValueDefinition domainValueDefinition, ImmutableDictionary<string, string> entities)
    {
        //todo: clean up
        // holy mess
        var firstSpaceIndex = propertySetup.BaseSource.IndexOf(' ');
        var initSource = propertySetup.BaseSource.Replace("set;", "init;");
        var foreignEntityName = domainValueDefinition.PropertyRelation.ForeignEntityName;

        if (domainValueDefinition.PropertyRelation.Type is RelationalType.None)
        {
            var entitySource = propertySetup.BaseSource.Insert(firstSpaceIndex, $" {domainValueDefinition.TypeName}?");
            var requestSource = propertySetup.BaseSource.Insert(firstSpaceIndex, $" {domainValueDefinition.RequestType}?");
            //    var commandSource = propertySetup.BaseSource.Insert(firstSpaceIndex, $" {domainValueDefinition.RequestType}");
            var responseSource = initSource.Insert(firstSpaceIndex, $" {domainValueDefinition.ResponseType}?");

            return new PropertySource
            {
                Entity = entitySource,
                Request = requestSource,
                //        Command = commandSource,
                MetadataType = domainValueDefinition.EntityType,
                Response = responseSource,
            };
        }

        if (domainValueDefinition.PropertyRelation.Type is RelationalType.ToOne)
        {
            var endOfPropertyNameIndex = propertySetup.BaseSource.IndexOf('{') - 1;
            var baseSource = propertySetup.BaseSource.Insert(endOfPropertyNameIndex, "Id");
            var entitySource = propertySetup.BaseSource.Insert(firstSpaceIndex, $" {domainValueDefinition.EntityType}");
            var requestSource = baseSource.Insert(firstSpaceIndex, $" {domainValueDefinition.RequestType}?");
            //    var commandSource = baseSource.Insert(firstSpaceIndex, $" {domainValueDefinition.RequestType}");
            var responseSource = initSource.Insert(firstSpaceIndex, $" {foreignEntityName}ResponseSimplified");
            var idSource = baseSource.Insert(firstSpaceIndex, $" {entities[domainValueDefinition.PropertyRelation.ForeignEntityName]}");
            var responseIdSource = initSource.Insert(firstSpaceIndex, $" string");

            return new PropertySource
            {
                Entity = entitySource,
                Request = requestSource,
                //        Command = commandSource,
                MetadataType = domainValueDefinition.EntityType,
                Response = responseSource,
                Id = idSource,
                ResponseId = responseIdSource
            };
        }

        if (domainValueDefinition.PropertyRelation.Type is RelationalType.ToMany)
        {
            var entitySource = propertySetup.BaseSource.Insert(firstSpaceIndex, $" {domainValueDefinition.EntityType}");
            var foreginResponseType = domainValueDefinition.EntityType.Replace(foreignEntityName, $"{foreignEntityName}ResponseSimplified");
            var responseIdSource = initSource.Insert(firstSpaceIndex, $" IEnumerable<string>");
            var responseSource = initSource.Insert(firstSpaceIndex, $" {foreginResponseType}").Replace("ICollection", "IEnumerable");
            var commandSource = propertySetup.BaseSource.Insert(firstSpaceIndex, $" IEnumerable<string>?");

            return new PropertySource
            {
                Entity = entitySource,
                MetadataType = domainValueDefinition.EntityType,
                Response = responseSource,
                ResponseId = responseIdSource,
                Request = commandSource,
            };
        }

        return default!;
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
                if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent) break;

                namespaceParent = parent;
                nameSpace = $"{namespaceParent.Name}.{nameSpace}";
            }
        }

        return nameSpace;
    }
}
