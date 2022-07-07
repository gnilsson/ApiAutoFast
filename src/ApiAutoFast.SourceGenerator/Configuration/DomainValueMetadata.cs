using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct DomainValueMetadata
{
    internal DomainValueMetadata(DomainValueDefinition domainValueDefinition, ImmutableArray<AttributeData> attributeDatas)
    {
        Definition = domainValueDefinition;
        AttributeDatas = attributeDatas;
    }

    internal readonly DomainValueDefinition Definition { get; }
    internal readonly ImmutableArray<AttributeData> AttributeDatas { get; }
}
