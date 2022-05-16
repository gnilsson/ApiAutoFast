using Microsoft.CodeAnalysis;

namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct DomainValueDefinition
{
    public DomainValueDefinition(ITypeSymbol requestType, ITypeSymbol entityType, ITypeSymbol responseType, string domainValueName)
    {
        RequestType = requestType;
        EntityType = entityType;
        ResponseType = responseType;
        DomainValueName = domainValueName;
    }

    internal readonly ITypeSymbol RequestType { get; }
    internal readonly ITypeSymbol EntityType { get; }
    internal readonly ITypeSymbol ResponseType { get; }
    internal readonly string DomainValueName { get; }
}
