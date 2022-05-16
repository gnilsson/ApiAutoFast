namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct DomainValueDefinition
{
    internal DomainValueDefinition(string requestType, string entityType, string responseType, string name)
    {
        RequestType = requestType;
        EntityType = entityType;
        ResponseType = responseType;
        Name = name;
    }

    internal readonly string RequestType { get; }
    internal readonly string EntityType { get; }
    internal readonly string ResponseType { get; }
    internal readonly string Name { get; }
}
