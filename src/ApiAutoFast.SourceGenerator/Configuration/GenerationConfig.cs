namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct GenerationConfig
{
    internal GenerationConfig(EntityGenerationConfig entityGeneration, ContextGenerationConfig? contextGeneration = null)
    {
        ContextGeneration = contextGeneration;
        EntityGeneration = entityGeneration;
    }

    internal static readonly GenerationConfig Empty = default;
    internal readonly ContextGenerationConfig? ContextGeneration { get; }
    internal readonly EntityGenerationConfig? EntityGeneration { get; }
}
