using ApiAutoFast.SourceGenerator.Configuration.Enums;
using System.Collections.Immutable;

namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct EntityConfig
{
    internal EntityConfig(AutoFastEndpointsAttributeArguments endpointsAttributeArguments, ImmutableDictionary<PropertyTarget, ImmutableArray<PropertyOutput>> properties, ImmutableArray<string> relationalNavigationNames)
    {
        BaseName = endpointsAttributeArguments.EntityName;
        Response = $"{endpointsAttributeArguments.EntityName}Response";
        MappingProfile = $"{endpointsAttributeArguments.EntityName}MappingProfile";
        CreateCommand = $"{endpointsAttributeArguments.EntityName}{nameof(RequestModelTarget.CreateCommand)}";
        ModifyCommand = $"{endpointsAttributeArguments.EntityName}{nameof(RequestModelTarget.ModifyCommand)}";
        EndpointsAttributeArguments = endpointsAttributeArguments;
        Properties = properties;
        RelationalNavigationNames = relationalNavigationNames;
    }

    internal readonly string BaseName { get; }
    internal readonly string Response { get; }
    internal readonly string MappingProfile { get; }
    internal readonly string CreateCommand { get; }
    internal readonly string ModifyCommand { get; }
    internal readonly AutoFastEndpointsAttributeArguments EndpointsAttributeArguments { get; }
    //internal readonly ImmutableArray<Property> Properties { get; }
    internal readonly ImmutableArray<string> RelationalNavigationNames { get; }
    internal readonly ImmutableDictionary<PropertyTarget, ImmutableArray<PropertyOutput>> Properties { get; }
}

internal readonly struct ModelOutput
{
    internal readonly IReadOnlyDictionary<PropertyTarget, ImmutableArray<string>> PropertyModelSource { get; }
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

internal enum PropertyTarget
{
    Entity = 0,
    CreateCommand,
    ModifyCommand,
    QueryRequest,
}
