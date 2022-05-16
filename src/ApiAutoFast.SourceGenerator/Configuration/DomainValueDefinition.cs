using Microsoft.CodeAnalysis;

namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct DomainValueDefinition
{
    internal DomainValueDefinition(string requestType, string entityType, string responseType, string domainValueName)
    {
        RequestType = requestType;
        EntityType = entityType;
        ResponseType = responseType;
        DomainValueName = domainValueName;
    }

    internal readonly string RequestType { get; }
    internal readonly string EntityType { get; }
    internal readonly string ResponseType { get; }
    internal readonly string DomainValueName { get; }
}
