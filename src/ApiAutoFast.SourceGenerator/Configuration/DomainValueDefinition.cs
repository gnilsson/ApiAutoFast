using Microsoft.CodeAnalysis;

namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct DomainValueDefinition
{
    public DomainValueDefinition(ITypeSymbol requestType, ITypeSymbol entityType, string domainValueName)
    {
        RequestType = requestType;
        EntityType = entityType;
        DomainValueName = domainValueName;
    }

    internal readonly ITypeSymbol RequestType { get; }
    internal readonly ITypeSymbol EntityType { get; }
    internal readonly string DomainValueName { get; }
}
