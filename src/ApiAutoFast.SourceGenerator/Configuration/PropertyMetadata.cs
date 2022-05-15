using ApiAutoFast.SourceGenerator.Configuration.Enums;
using System.Collections.Immutable;

namespace ApiAutoFast.SourceGenerator.Configuration;

// note: should probably restructure this to have propertymetadata of type entitymodel and requestmodel insted of making the distinction twice on attributes and source
internal readonly struct PropertyMetadata
{
    internal PropertyMetadata(
        string entitySource,
        string requestSource,
        string name,
        DomainValueDefinition domainValueDefiniton,
        RequestModelTarget requestModelTarget,
        ImmutableArray<PropertyAttributeMetadata>? attributeMetadatas = null,
        PropertyRelational? relational = null)
    {
        EntitySource = entitySource;
        RequestSource = requestSource;
        Name = name;
        DomainValueDefiniton = domainValueDefiniton;
        AttributeMetadatas = attributeMetadatas;
        RequestModelTarget = requestModelTarget;
        Relational = relational;
    }

    internal readonly string RequestSource { get; }
    internal readonly string EntitySource { get; }
    internal readonly string Name { get; }
    internal readonly DomainValueDefinition DomainValueDefiniton { get; }
    internal readonly ImmutableArray<PropertyAttributeMetadata>? AttributeMetadatas { get; }
    internal readonly RequestModelTarget RequestModelTarget { get; }
    internal readonly PropertyRelational? Relational { get; }
}
