namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct ContextGenerationConfig
{
    internal ContextGenerationConfig(string name)
    {
        Name = name;
    }

    internal readonly string Name { get; }
}
