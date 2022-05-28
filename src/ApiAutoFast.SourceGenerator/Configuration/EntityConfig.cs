using ApiAutoFast.SourceGenerator.Configuration.Enums;
using System.Collections.Immutable;

namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct EntityConfig
{
    internal EntityConfig(AutoFastEndpointsAttributeArguments endpointsAttributeArguments, ImmutableArray<Property> properties, ImmutableArray<string> relationalNavigationNames)
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
    internal readonly ImmutableArray<Property> Properties { get; }
    internal readonly ImmutableArray<string> RelationalNavigationNames { get; }
}

internal readonly struct ModelOutput
{
    internal readonly IReadOnlyDictionary<PropertyTarget, ImmutableArray<string>> PropertyModelSource { get; }
}

internal readonly struct PropertyOutput
{
    public PropertyOutput(string entityType, string source, PropertyTarget target)
    {
        EntityType = entityType;
        Source = source;
        Target = target;
    }

    internal readonly string EntityType { get; }
    internal readonly string Source { get; }
    internal readonly PropertyTarget Target { get; }
}


internal readonly struct Property
{
    public Property(string name, ImmutableArray<PropertyOutput> propertyOutputs, PropertyRelation relation)
    {
        Entity = entity;
        Name = name;
        PropertyOutputs = propertyOutputs;
        Relation = relation;
    }

    public string Entity { get; }
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
