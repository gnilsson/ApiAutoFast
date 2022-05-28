using ApiAutoFast.SourceGenerator.Configuration;
using ApiAutoFast.SourceGenerator.Configuration.Enums;
using ApiAutoFast.SourceGenerator.Descriptive;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

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

            //var members = semanticModel
            //    .GetDeclaredSymbol(entityClassDeclaration)!
            //    .GetMembers();

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

    //private static IEnumerable<KeyValuePair<string, ImmutableArray<Property>>> YieldPropertySetup3(Compilation compilation, ImmutableArray<EntityConfigSetup> entityConfigSetups)
    //{
    //    var entityNames = entityConfigSetups
    //        .Select(x => x.EndpointsAttributeArguments.EntityName)
    //        .ToImmutableArray();

    //    foreach (var entity in entityConfigSetups)
    //    {
    //        var propertySetups = YieldProperty(compilation, entityNames, entity).ToImmutableArray();

    //        yield return new KeyValuePair<string, ImmutableArray<Property>>(entity.EndpointsAttributeArguments.EntityName, propertySetups);

    //        //yield return new KeyValuePair<string, IReadOnlyDictionary<DomainValueDefinition, ImmutableArray<PropertySetup2>>>(
    //        //    entity.EndpointsAttributeArguments.EntityName,
    //        //    new ReadOnlyDictionary<DomainValueDefinition, ImmutableArray<PropertySetup2>>(propertySetups));
    //    }
    //}

    private static IEnumerable<IEnumerable<Property>> YieldPropertySetup3(Compilation compilation, ImmutableArray<EntityConfigSetup> entityConfigSetups)
    {
        var entityNames = entityConfigSetups
            .Select(x => x.EndpointsAttributeArguments.EntityName)
            .ToImmutableArray();

        foreach (var entity in entityConfigSetups)
        {
            yield return YieldProperty(compilation, entityNames, entity);

           // yield return new KeyValuePair<string, ImmutableArray<Property>>(entity.EndpointsAttributeArguments.EntityName, propertySetups);

            //yield return new KeyValuePair<string, IReadOnlyDictionary<DomainValueDefinition, ImmutableArray<PropertySetup2>>>(
            //    entity.EndpointsAttributeArguments.EntityName,
            //    new ReadOnlyDictionary<DomainValueDefinition, ImmutableArray<PropertySetup2>>(propertySetups));
        }
    }

    //private static IEnumerable<KeyValuePair<DomainValueDefinition, ImmutableArray<PropertySetup2>>> YieldPropertySetup(Compilation compilation, ImmutableArray<string> entityNames, EntityConfigSetup entity)
    //{
    private static IEnumerable<Property> YieldProperty(Compilation compilation, ImmutableArray<string> entityNames, EntityConfigSetup entity)
    {
        var members = entity.SemanticModel
            .GetDeclaredSymbol(entity.ClassDeclarationSyntax)!
            .GetMembers();

        var found = new List<DomainValueDefinition>();

        foreach (var member in members)
        {
            if (member is not IPropertySymbol property) continue;

            if (TryGetDomainValueDefinition(compilation, property, entityNames, out var domainValueDefinition) is false)
            {
                continue;
            }

            var hmm = YieldPropertySetup2(entity).ToImmutableArray();

            var f = new Property(entity.EndpointsAttributeArguments.EntityName, property.Name, hmm, domainValueDefinition.PropertyRelation);

            yield return f;

            IEnumerable<PropertyOutput> YieldPropertySetup2(EntityConfigSetup entity)
            {
                var propertyString = property.ToDisplayString(new SymbolDisplayFormat(
                    propertyStyle: SymbolDisplayPropertyStyle.ShowReadWriteDescriptor,
                    memberOptions: SymbolDisplayMemberOptions.IncludeAccessibility));

                if (found.Contains(domainValueDefinition) is false)
                {
                    //var semanticModel = compilation.GetSemanticModel(entity.ClassDeclarationSyntax.SyntaxTree);

                    //if (semanticModel.GetDeclaredSymbol(entity.ClassDeclarationSyntax) is not INamedTypeSymbol namedTypeSymbol) continue;

                    //var attributes = namedTypeSymbol.GetAttributes();

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
                                yield return new PropertyOutput(include, propertyString, PropertyTarget.CreateCommand);
                            }
                        }
                    }
                }

                if (domainValueDefinition.PropertyRelation.Type is RelationalType.ToOne)
                {
                    // note: this got way too messy when its no direct 1-1 mapping from real and generated properties..
                    var extendPropertyNameIndex = propertyString.IndexOf('{') - 1;
                    var idPropertyString = propertyString.Insert(extendPropertyNameIndex, $"Id");

                    yield return new PropertyOutput(entity.EndpointsAttributeArguments.EntityName, idPropertyString, PropertyTarget.Entity);
                }

                var firstSpaceIndex = propertyString.IndexOf(' ');

                var entitySource = domainValueDefinition.PropertyRelation.Type is RelationalType.None
                    ? propertyString.Insert(firstSpaceIndex, $" {domainValueDefinition.TypeName}")
                    : propertyString.Insert(firstSpaceIndex, $" {domainValueDefinition.EntityType}");

                yield return new PropertyOutput(entity.EndpointsAttributeArguments.EntityName, entitySource, PropertyTarget.Entity);

                if (domainValueDefinition.PropertyRelation.Type is not RelationalType.ToMany)
                {
                    var requestSource = propertyString.Insert(firstSpaceIndex, $" {domainValueDefinition.RequestType}?");
                    var commandSource = propertyString.Insert(firstSpaceIndex, $" {domainValueDefinition.RequestType}");

                    yield return new PropertyOutput(entity.EndpointsAttributeArguments.EntityName, requestSource, PropertyTarget.QueryRequest);
                    yield return new PropertyOutput(entity.EndpointsAttributeArguments.EntityName, commandSource, PropertyTarget.CreateCommand);
                    yield return new PropertyOutput(entity.EndpointsAttributeArguments.EntityName, commandSource, PropertyTarget.ModifyCommand);
                }

            }


        }
    }

    private readonly struct DomainValue
    {
        public DomainValue()
        {

        }
    }

    //private readonly struct DomainValueDefinitionReference
    //{
    //    public DomainValueDefinitionReference(DomainValueDefinition domainValueDefinition, string entityName, ClassDeclarationSyntax classDeclarationSyntax)
    //    {
    //        DomainValueDefinition = domainValueDefinition;
    //        EntityName = entityName;
    //        ClassDeclarationSyntax = classDeclarationSyntax;
    //    }

    //    internal readonly DomainValueDefinition DomainValueDefinition { get; }
    //    internal readonly string EntityName { get; }
    //    public ClassDeclarationSyntax ClassDeclarationSyntax { get; }
    //}

    //private static IEnumerable<PropertySetup2> Aha(ImmutableArray<DomainValueDefinitionReference> domainValueDefinitionReferences)
    //{
    //    foreach (var reference in domainValueDefinitionReferences)
    //    {
    //        if (semanticModel.GetDeclaredSymbol(reference.entityClassDeclaration) is not INamedTypeSymbol namedTypeSymbol) continue;


    //        yield return new PropertySetup2(reference.DomainValueDefinition.RequestType, "", PropertyTarget.QueryRequest);
    //    }
    //}


    private static IEnumerable<EntityConfig> YieldEntityConfig(
        Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax> entityClassDeclarations,
        CancellationToken ct)
    {
        var entityConfigSetups = YieldEntityConfigSetup(compilation, entityClassDeclarations).ToImmutableArray();

        var ff = YieldPropertySetup3(compilation, entityConfigSetups).SelectMany(x => x.GroupBy(y => y.PropertyOutputs.Select(x => x.EntityType)));

        foreach (var ss in ff)
        {
            var relationalNavigationNames = ss
                    .Where(x => x.Relation.Type is not RelationalType.None)
                    .Select(x => x.Relation.ForeigEntityProperty)
                    .ToImmutableArray();

            var endpointsAttributeArguments = entityConfigSetups
                .First(x => x.EndpointsAttributeArguments.EntityName == ss.FirstOrDefault().PropertyOutputs.First().EntityType)
                .EndpointsAttributeArguments;

            var ahh = ss.Select(x => x).ToImmutableArray();

            yield return new EntityConfig(endpointsAttributeArguments, ahh, relationalNavigationNames);
        }

        //var domainValueProperties = YieldPropertySetup3(compilation, entityConfigSetups).ToDictionary(x => x.Key, x => x.Value);

        //foreach (var aha in domainValueProperties)
        //{
        //    var relationalNavigationNames = aha.Value
        //        .Where(x => x.Relation.Type is not RelationalType.None)
        //        .Select(x => x.Relation.ForeigEntityProperty)
        //        .ToImmutableArray();

        //    var endpointsAttributeArguments = entityConfigSetups
        //        .First(x => x.EndpointsAttributeArguments.EntityName == aha.Key)
        //        .EndpointsAttributeArguments;

        //    var ff = aha.Value.SelectMany(x => x.PropertyOutputs.Where(y => y.Target is PropertyTarget.Entity));

        //    yield return new EntityConfig(endpointsAttributeArguments, aha.Value, relationalNavigationNames);
        //}




        foreach (var item in entityConfigSetups)
        {
            var f = domainValueProperties.Values.Where(x => x.Select(x => x.EntityType) == item.EndpointsAttributeArguments.EntityName));

            //var group = domainValueProperties.GroupBy(x => x.Value.Select(x =))

            //var f = domainValueProperties.Where(x => x.Value.)
            //    .FirstOrDefault(x => (x.Key, x.Value.Where(x => x.EntityType == item.EndpointsAttributeArguments.EntityName)));


            var relationalNavigationNames = domainValueProperties
                .Where(x => x.Value.Select(x => x.EntityType).Contains(item.EndpointsAttributeArguments.EntityName) && x.Key.PropertyRelation.RelationalType is not RelationalType.None)
                .Select(x => x.Key.PropertyRelation.ForeigEntityProperty)
                .ToImmutableArray();


        }

        foreach (var setup in domainValueProperties)
        {


            yield return new EntityConfig(entityConfigSetup.EndpointsAttributeArguments, propertyMetadatas, relationalNavigationNames);
        }

        //var groups = propertySetups.GroupBy(x => x.EntityType);

        //foreach (var group in groups)
        //{
        //    var entityPropertySetups = group.Select(x => x).ToImmutableArray();

        //    //var propertyMetadatas = YieldPropertyMetadata(propertySetups).ToImmutableArray();
        //}

        //foreach (var entityConfigSetup in entityConfigSetups)
        //{
        //    ct.ThrowIfCancellationRequested();



        //    var propertySetups = YieldPropertySetup(compilation, members, foreignEntityNames, entityConfigSetup.EndpointsAttributeArguments.EntityName).ToImmutableArray();

        //    propertySetups = propertySetups.Distinct().ToImmutableArray();

        //    var propertyMetadatas = YieldPropertyMetadata(propertySetups).ToImmutableArray();

        //    var relationalNavigationNames = propertyMetadatas
        //        .Where(x => x.DomainValueDefiniton.PropertyRelation.RelationalType is not RelationalType.None)
        //        .Select(x => x.DomainValueDefiniton.PropertyRelation.ForeigEntityProperty)
        //        .ToImmutableArray();

        //    yield return new EntityConfig(entityConfigSetup.EndpointsAttributeArguments, propertyMetadatas, relationalNavigationNames);
        //}
    }

    //private static IEnumerable<EntityConfig> YieldEntityConfig(
    //    Compilation compilation,
    //    ImmutableArray<ClassDeclarationSyntax> entityClassDeclarations,
    //    CancellationToken ct)
    //{
    //    var entityConfigSetups = YieldEntityConfigSetup(compilation, entityClassDeclarations).ToImmutableArray();

    //    // note: some are duplicates
    //    //var domainValueDefinitions = YieldDomainValueDefinitionReference(compilation, entityConfigSetups).ToImmutableArray();

    //    var propertySetups = YieldPropertySetup(compilation, entityConfigSetups).ToImmutableArray();

    //    foreach (var entityConfigSetup in entityConfigSetups)
    //    {
    //        ct.ThrowIfCancellationRequested();

    //        var members = entityConfigSetup.SemanticModel
    //            .GetDeclaredSymbol(entityConfigSetup.ClassDeclarationSyntax)!
    //            .GetMembers();

    //        var foreignEntityNames = entityConfigSetups
    //            .Where(x => x.EndpointsAttributeArguments.EntityName != entityConfigSetup.EndpointsAttributeArguments.EntityName)
    //            .Select(x => x.EndpointsAttributeArguments.EntityName)
    //            .ToImmutableArray();

    //        var propertySetups = YieldPropertySetup(compilation, members, foreignEntityNames, entityConfigSetup.EndpointsAttributeArguments.EntityName).ToImmutableArray();

    //        propertySetups = propertySetups.Distinct().ToImmutableArray();

    //        var propertyMetadatas = YieldPropertyMetadata(propertySetups).ToImmutableArray();

    //        var relationalNavigationNames = propertyMetadatas
    //            .Where(x => x.DomainValueDefiniton.PropertyRelation.RelationalType is not RelationalType.None)
    //            .Select(x => x.DomainValueDefiniton.PropertyRelation.ForeigEntityProperty)
    //            .ToImmutableArray();

    //        yield return new EntityConfig(entityConfigSetup.EndpointsAttributeArguments, propertyMetadatas, relationalNavigationNames);
    //    }
    //}


    //private readonly struct PropertySetup
    //{
    //    public PropertySetup(DomainValueDefinition domainValueDefinition, PropertyLocation entityLocation, string basePropertyString)
    //    {
    //        DomainValueDefinition = domainValueDefinition;
    //        EntityLocation = entityLocation;
    //        BasePropertyString = basePropertyString;
    //    }

    //    internal readonly DomainValueDefinition DomainValueDefinition { get; }
    //    internal readonly PropertyLocation EntityLocation { get; }
    //    internal readonly string BasePropertyString { get; }
    //}

    //private readonly struct PropertyLocation
    //{
    //    public PropertyLocation(string name, PropertyTarget propertyTarget)
    //    {
    //        Name = name;
    //        PropertyTarget = propertyTarget;
    //    }

    //    internal readonly string Name { get; }
    //    internal readonly PropertyTarget PropertyTarget { get; }
    //}

    //private static IEnumerable<PropertySetup> YieldPropertySetup(Compilation compilation, ImmutableArray<ISymbol> members, ImmutableArray<string> foreignEntityNames, string entityName)
    //{
    //    var dict = new Dictionary<PropertyLocation, PropertySetup>();

    //    foreach (var member in members)
    //    {
    //        if (member is not IPropertySymbol property
    //            || TryGetDomainValueDefinition(compilation, property, foreignEntityNames, out var domainValueDefinition) is false)
    //        {
    //            continue;
    //        }



    //        var propertyString = property.ToDisplayString(new SymbolDisplayFormat(
    //            propertyStyle: SymbolDisplayPropertyStyle.ShowReadWriteDescriptor,
    //            memberOptions: SymbolDisplayMemberOptions.IncludeAccessibility));


    //        yield return new PropertySetup(domainValueDefinition, new PropertyLocation(entityName, PropertyTarget.CreateCommand), propertyString);
    //        yield return new PropertySetup(domainValueDefinition, new PropertyLocation(entityName, PropertyTarget.ModifyCommand), propertyString);
    //        yield return new PropertySetup(domainValueDefinition, new PropertyLocation(entityName, PropertyTarget.QueryRequest), propertyString);
    //    }
    //}

    //private static IEnumerable<PropertyMetadata> YieldPropertyMetadata(ImmutableArray<PropertySetup2> propertySetups)
    //{
    //    foreach (var setup in propertySetups)
    //    {
    //        var propertyString = setup.BaseSource;
    //        //var property = setup.PropertySymbol;
    //        //var domainValueDefinition = setup.DomainValueDefinition;

    //        //var propertyString = property.ToDisplayString(new SymbolDisplayFormat(
    //        //    propertyStyle: SymbolDisplayPropertyStyle.ShowReadWriteDescriptor,
    //        //    memberOptions: SymbolDisplayMemberOptions.IncludeAccessibility));

    //        var firstSpaceIndex = propertyString.IndexOf(' ');
    //        // note: might consider using domainvalue.entitytype here to skip the whole valueconversion shenanigans
    //        var entitySource = domainValueDefinition.PropertyRelation.RelationalType is RelationalType.None
    //            ? propertyString.Insert(firstSpaceIndex, $" {domainValueDefinition.TypeName}")
    //            : propertyString.Insert(firstSpaceIndex, $" {domainValueDefinition.EntityType}");

    //        if (domainValueDefinition.PropertyRelation.RelationalType is RelationalType.ToOne)
    //        {
    //            // note: this got way too messy when its no direct 1-1 mapping from real and generated properties..
    //            var extendPropertyNameIndex = propertyString.IndexOf('{') - 1;
    //            propertyString = propertyString.Insert(extendPropertyNameIndex, $"Id");
    //        }

    //        var requestSource = domainValueDefinition.PropertyRelation.RelationalType is RelationalType.ToMany
    //            ? string.Empty
    //            : propertyString.Insert(firstSpaceIndex, $" {domainValueDefinition.RequestType}?");

    //        var commandSource = domainValueDefinition.PropertyRelation.RelationalType is RelationalType.ToMany
    //            ? string.Empty // note: as of now there is no support to adding multiple foreign entities in one command
    //            : propertyString.Insert(firstSpaceIndex, $" {domainValueDefinition.RequestType}");

    //        var attributes = YieldAttributeMetadata(property).ToImmutableArray();

    //        var requestModelTarget = GetRequestModelTarget(attributes);

    //        yield return new PropertyMetadata(entitySource, requestSource, commandSource, domainValueDefinition, requestModelTarget, attributes);
    //    }
    //}

    //private static IEnumerable<PropertyMetadata> YieldPropertyMetadata(Compilation compilation, ImmutableArray<ISymbol> members, ImmutableArray<string> foreignEntityNames)
    //{
    //    foreach (var member in members)
    //    {
    //        if (member is not IPropertySymbol property) continue;

    //        if (TryGetDomainValueDefinition(compilation, property, foreignEntityNames, out var domainValueDefinition) is false) continue;

    //        var propertyString = property.ToDisplayString(new SymbolDisplayFormat(
    //            propertyStyle: SymbolDisplayPropertyStyle.ShowReadWriteDescriptor,
    //            memberOptions: SymbolDisplayMemberOptions.IncludeAccessibility));

    //        var firstSpaceIndex = propertyString.IndexOf(' ');
    //        // note: might consider using domainvalue.entitytype here to skip the whole valueconversion shenanigans
    //        var entitySource = domainValueDefinition.PropertyRelation.RelationalType is RelationalType.None
    //            ? propertyString.Insert(firstSpaceIndex, $" {domainValueDefinition.TypeName}")
    //            : propertyString.Insert(firstSpaceIndex, $" {domainValueDefinition.EntityType}");

    //        if (domainValueDefinition.PropertyRelation.RelationalType is RelationalType.ToOne)
    //        {
    //            // note: this got way too messy when its no direct 1-1 mapping from real and generated properties..
    //            var extendPropertyNameIndex = propertyString.IndexOf('{') - 1;
    //            propertyString = propertyString.Insert(extendPropertyNameIndex, $"Id");
    //        }

    //        var requestSource = domainValueDefinition.PropertyRelation.RelationalType is RelationalType.ToMany
    //            ? string.Empty
    //            : propertyString.Insert(firstSpaceIndex, $" {domainValueDefinition.RequestType}?");

    //        var commandSource = domainValueDefinition.PropertyRelation.RelationalType is RelationalType.ToMany
    //            ? string.Empty // note: as of now there is no support to adding multiple foreign entities in one command
    //            : propertyString.Insert(firstSpaceIndex, $" {domainValueDefinition.RequestType}");

    //        var attributes = YieldAttributeMetadata(property).ToImmutableArray();

    //        var requestModelTarget = GetRequestModelTarget(attributes);

    //        yield return new PropertyMetadata(entitySource, requestSource, commandSource, domainValueDefinition, requestModelTarget, attributes);
    //    }
    //}

    private static bool TryGetDomainValueDefinition(
        Compilation compilation,
        IPropertySymbol property,
        ImmutableArray<string> entityNames,
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

            var propertyRelation = GetPropertyRelation(property, entityNames, entityType);

            var typeName = propertyRelation.Type == RelationalType.ToOne ? TypeText.Identifier : property.Type.Name;

            var responseType = domainValueType.BaseType.TypeArguments.Length == 4
                ? domainValueType.BaseType.TypeArguments[2]
                : requestType;

            domainValueDefinition = new DomainValueDefinition(
                domainValueType,
                requestType.ToString(),
                entityType.ToString(),
                responseType.ToString(),
                property.Name,
                typeName,
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

            var attributeName = GetLastPart(attributeData.AttributeClass.Name).Replace(TypeText.Attribute, "");

            if (attributeName == TypeText.AttributeText.ExcludeRequestModel)
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
