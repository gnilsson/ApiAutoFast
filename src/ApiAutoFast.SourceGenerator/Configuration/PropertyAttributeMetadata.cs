using ApiAutoFast.SourceGenerator.Configuration.Enums;

namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct PropertyAttributeMetadata
{
    internal PropertyAttributeMetadata(
        AttributeType attributeType,
        string name,
        RequestModelTarget? requestModelTarget = null)
    {
        AttributeType = attributeType;
        Name = name;
        RequestModelTarget = requestModelTarget;
    }

    public AttributeType AttributeType { get; }
    public string Name { get; }
    public RequestModelTarget? RequestModelTarget { get; }
}
