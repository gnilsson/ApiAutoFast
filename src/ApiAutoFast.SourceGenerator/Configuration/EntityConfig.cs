using ApiAutoFast.SourceGenerator.Configuration.Enums;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct EntityConfig
{
    internal EntityConfig(AutoFastEndpointsAttributeArguments endpointsAttributeArguments, ImmutableArray<string> relationalNavigationNames, PropertyConfig propertyConfig)
    {
        BaseName = endpointsAttributeArguments.EntityName;
        Response = $"{endpointsAttributeArguments.EntityName}Response";
        MappingProfile = $"{endpointsAttributeArguments.EntityName}MappingProfile";
        CreateCommand = $"{endpointsAttributeArguments.EntityName}{nameof(RequestModelTarget.CreateCommand)}";
        ModifyCommand = $"{endpointsAttributeArguments.EntityName}{nameof(RequestModelTarget.ModifyCommand)}";
        EndpointsAttributeArguments = endpointsAttributeArguments;
        RelationalNavigationNames = relationalNavigationNames;
        PropertyConfig = propertyConfig;
    }

    internal readonly string BaseName { get; }
    internal readonly string Response { get; }
    internal readonly string MappingProfile { get; }
    internal readonly string CreateCommand { get; }
    internal readonly string ModifyCommand { get; }
    internal readonly AutoFastEndpointsAttributeArguments EndpointsAttributeArguments { get; }
    internal readonly ImmutableArray<string> RelationalNavigationNames { get; }
    internal readonly PropertyConfig PropertyConfig { get; }
}

internal readonly struct PropertyConfig
{
    internal PropertyConfig(string entityName, ImmutableArray<PropertyOutput> properties, ImmutableArray<DefinedDomainValue> domainValues)
    {
        EntityName = entityName;
        Properties = properties;
        DomainValues = domainValues;
    }

    internal readonly string EntityName { get; }
    internal readonly ImmutableArray<PropertyOutput> Properties { get; }
    internal readonly ImmutableArray<DefinedDomainValue> DomainValues { get; }
}

internal readonly struct DomainValueMetadata
{
    internal DomainValueMetadata(DomainValueDefinition domainValueDefinition, ImmutableArray<AttributeData> attributeDatas)
    {
        DomainValueDefinition = domainValueDefinition;
        AttributeDatas = attributeDatas;
    }

    internal readonly DomainValueDefinition DomainValueDefinition { get; }
    internal readonly ImmutableArray<AttributeData> AttributeDatas { get; }
}


internal readonly struct PropertySetup
{
    internal PropertySetup(string name, string baseSource, DomainValueMetadata domainValueMetadata)
    {
        Name = name;
        BaseSource = baseSource;
        DomainValueMetadata = domainValueMetadata;
    }

    internal readonly string Name { get; }
    internal readonly string BaseSource { get; }
    internal readonly DomainValueMetadata DomainValueMetadata { get; }
}

internal readonly struct DefinedProperty
{
    public DefinedProperty(string name, PropertyKind propertyKind)
    {
        Name = name;
        PropertyKind = propertyKind;
    }

    internal readonly string Name { get; }
    internal readonly PropertyKind PropertyKind { get; }
}

internal readonly struct DefinedDomainValue
{
    public DefinedDomainValue(DomainValueDefinition domainValueDefinition, ImmutableArray<DefinedProperty> definedProperties)
    {
        DomainValueDefinition = domainValueDefinition;
        DefinedProperties = definedProperties;
    }

    public DomainValueDefinition DomainValueDefinition { get; }
    public ImmutableArray<DefinedProperty> DefinedProperties { get; }
}

internal readonly struct DomainValue
{
    public DomainValue(string entityType, string requestType, PropertyRelation propertyRelation)
    {
        EntityType = entityType;
        RequestType = requestType;
        PropertyRelation = propertyRelation;
    }

    public string EntityType { get; }
    public string RequestType { get; }
    public PropertyRelation PropertyRelation { get; }
}

internal readonly struct DomainValueProperty
{
    public DomainValueProperty(string propertyName, string entityName, DomainValue domainValue)
    {
        PropertyName = propertyName;
        EntityName = entityName;
        DomainValue = domainValue;
    }

    public string PropertyName { get; }
    public string EntityName { get; }
    public DomainValue DomainValue { get; }
}


internal readonly struct PropertyOutput
{
    public PropertyOutput(string entityKind, string source, PropertyTarget target, string name, string type, PropertyRelation? relation = null, PropertyKind propertyKind = PropertyKind.Domain)
    {
        EntityKind = entityKind;
        Source = source;
        Target = target;
        Name = name;
        Type = type;
        Relation = relation ?? PropertyRelation.None;
        PropertyKind = propertyKind;
    }

    internal readonly string EntityKind { get; }
    internal readonly string Source { get; }
    internal readonly PropertyTarget Target { get; }
    public string Name { get; }
    public string Type { get; }
    public PropertyRelation Relation { get; }
    public PropertyKind PropertyKind { get; }
}


internal readonly struct Property
{
    public Property(string name, ImmutableArray<PropertyOutput> propertyOutputs, PropertyRelation relation)
    {
        Name = name;
        PropertyOutputs = propertyOutputs;
        Relation = relation;
    }

  //  public string Entity { get; }
    public string Name { get; }
    public ImmutableArray<PropertyOutput> PropertyOutputs { get; }
    public PropertyRelation Relation { get; }
}

[Flags]
internal enum PropertyTarget
{
    None = 0,
    Entity = 1,
    CreateCommand = 2,
    ModifyCommand = 4,
    QueryRequest = 8,
    All = 16,
}
