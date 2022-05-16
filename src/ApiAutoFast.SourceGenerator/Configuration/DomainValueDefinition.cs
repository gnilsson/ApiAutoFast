namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct DomainValueDefinition
{
    internal DomainValueDefinition(string requestType, string entityType, string responseType, string name, bool requestIsValueType, string? valueTypeDefaultValue = null)
    {
        RequestType = requestType;
        EntityType = entityType;
        ResponseType = responseType;
        Name = name;
        RequestIsValueType = requestIsValueType;
        ValueTypeDefaultValue = valueTypeDefaultValue;
    }

    internal readonly string RequestType { get; }
    internal readonly string EntityType { get; }
    internal readonly string ResponseType { get; }
    internal readonly string Name { get; }
    internal readonly bool RequestIsValueType { get; }
    internal readonly string? ValueTypeDefaultValue { get; }
}
