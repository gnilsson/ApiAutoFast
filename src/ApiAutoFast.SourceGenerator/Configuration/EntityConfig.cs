using System.Collections.Immutable;

namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct EntityConfig
{
    internal EntityConfig(string name, in ImmutableArray<PropertyMetadata>? propertyMetadatas = null)
    {
        BaseName = name;
        Response = $"{name}Response";
        MappingProfile = $"{name}MappingProfile";
        PropertyMetadatas = propertyMetadatas;
    }

    internal readonly string BaseName { get; }
    internal readonly string Response { get; }
    internal readonly string MappingProfile { get; }
    internal readonly ImmutableArray<PropertyMetadata>? PropertyMetadatas { get; }
}
