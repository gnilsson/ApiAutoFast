using System.Collections.Immutable;

namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct DefinedDomainValue
{
    public DefinedDomainValue(DomainValueDefinition domainValueDefinition, ImmutableArray<DefinedProperty> definedProperties)
    {
        DomainValueDefinition = domainValueDefinition;
        DefinedProperties = definedProperties;
    }

    public DomainValueDefinition DomainValueDefinition { get; }
    public ImmutableArray<DefinedProperty> DefinedProperties { get; }
}
