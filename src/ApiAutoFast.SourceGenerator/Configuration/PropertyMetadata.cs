using ApiAutoFast.SourceGenerator.Configuration.Enums;
using System.Collections.Immutable;

namespace ApiAutoFast.SourceGenerator.Configuration;

// note: should probably restructure this to have propertymetadata of type entitymodel and requestmodel insted of making the distinction twice on attributes and source
internal readonly struct PropertyMetadata
{
    internal PropertyMetadata(
        string entitySource,
        string requestSource,
        string commandSource,
        DomainValueDefinition domainValueDefiniton,
        RequestModelTarget requestModelTarget,
        ImmutableArray<PropertyAttributeMetadata> attributeMetadatas)
    {
        EntitySource = entitySource;
        RequestSource = requestSource;
        CommandSource = commandSource;
        DomainValueDefiniton = domainValueDefiniton;
        AttributeMetadatas = attributeMetadatas;
        RequestModelTarget = requestModelTarget;
    }

    internal readonly string RequestSource { get; }
    internal readonly string EntitySource { get; }
    internal readonly string CommandSource { get; }
    internal readonly DomainValueDefinition DomainValueDefiniton { get; }
    internal readonly ImmutableArray<PropertyAttributeMetadata> AttributeMetadatas { get; }
    internal readonly RequestModelTarget RequestModelTarget { get; }
}
