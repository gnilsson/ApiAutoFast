using System.Collections.Immutable;

namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct EntityGenerationConfig
{
    internal EntityGenerationConfig(ImmutableArray<EntityConfig> entityConfigs, string @namespace, ImmutableArray<string> targetedEndpointNames)
    {
        EntityConfigs = entityConfigs;
        Namespace = @namespace;
        TargetedEndpointNames = targetedEndpointNames;
    }

    internal readonly ImmutableArray<EntityConfig> EntityConfigs { get; }
    internal readonly string Namespace { get; }
    internal readonly ImmutableArray<string> TargetedEndpointNames { get; }
}
