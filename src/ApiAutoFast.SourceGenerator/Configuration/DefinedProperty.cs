using ApiAutoFast.SourceGenerator.Configuration.Enums;

namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct DefinedProperty
{
    public DefinedProperty(string name, PropertyKind propertyKind, string type)
    {
        Name = name;
        PropertyKind = propertyKind;
        Type = type;
    }

    internal readonly string Name { get; }
    internal readonly PropertyKind PropertyKind { get; }
    internal readonly string Type { get; }
}
