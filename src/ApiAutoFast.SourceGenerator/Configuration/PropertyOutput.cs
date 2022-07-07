using ApiAutoFast.SourceGenerator.Configuration.Enums;

namespace ApiAutoFast.SourceGenerator.Configuration;

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
