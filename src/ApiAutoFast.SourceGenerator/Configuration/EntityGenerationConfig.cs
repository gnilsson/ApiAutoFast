using System.Collections.Immutable;

namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct EntityGenerationConfig
{
    internal EntityGenerationConfig(ImmutableArray<EntityConfig> entityConfigs, string @namespace)
    {
        EntityConfigs = entityConfigs;
        Namespace = @namespace;
    }

    internal readonly ImmutableArray<EntityConfig> EntityConfigs { get; }
    internal readonly string Namespace { get; }
}
