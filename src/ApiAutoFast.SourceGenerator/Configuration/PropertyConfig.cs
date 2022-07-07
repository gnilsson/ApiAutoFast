using System.Collections.Immutable;

namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct PropertyConfig
{
    internal PropertyConfig(string entityName, ImmutableArray<PropertyOutput> properties, ImmutableArray<DefinedDomainValue> domainValues)
    {
        EntityName = entityName;
        Properties = properties;
        DomainValues = domainValues;
    }

    internal readonly string EntityName { get; }
    internal readonly ImmutableArray<PropertyOutput> Properties { get; }
    internal readonly ImmutableArray<DefinedDomainValue> DomainValues { get; }
}
