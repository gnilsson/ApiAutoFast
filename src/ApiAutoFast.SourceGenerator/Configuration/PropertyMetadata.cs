﻿using ApiAutoFast.SourceGenerator.Configuration.Enums;
using System.Collections.Immutable;

namespace ApiAutoFast.SourceGenerator.Configuration;

// note: should probably restructure this to have propertymetadata of type entitymodel and requestmodel insted of making the distinction twice on attributes and source
internal readonly struct PropertyMetadata
{
    internal PropertyMetadata(
        PropertySource source,
        string name,
        bool isEnum,
        RequestModelTarget requestModelTarget,
        ImmutableArray<PropertyAttributeMetadata>? attributeMetadatas = null,
        PropertyRelational? relational = null)
    {
        Source = source;
        Name = name;
        AttributeMetadatas = attributeMetadatas;
        IsEnum = isEnum;
        RequestModelTarget = requestModelTarget;
        Relational = relational;
    }

    internal readonly PropertySource Source { get; }
    internal readonly string Name { get; }
    internal readonly ImmutableArray<PropertyAttributeMetadata>? AttributeMetadatas { get; }
    internal readonly RequestModelTarget RequestModelTarget { get; }
    internal readonly bool IsEnum { get; }
    internal readonly PropertyRelational? Relational { get; }
}