using ApiAutoFast.SourceGenerator.Configuration.Enums;

namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct PropertyOutput
{
    public PropertyOutput(string entityKind, string source, PropertyTarget target, string name, PropertyRelation? relation = null, PropertyKind propertyKind = PropertyKind.Domain)
    {
        EntityKind = entityKind;
        Source = source;
        Target = target;
        Name = name;
        Relation = relation ?? PropertyRelation.None;
        PropertyKind = propertyKind;
    }

    internal readonly string EntityKind { get; }
    internal readonly string Source { get; }
    internal readonly PropertyTarget Target { get; }
    internal readonly string Name { get; }
    internal readonly PropertyRelation Relation { get; }
    internal readonly PropertyKind PropertyKind { get; }
}
