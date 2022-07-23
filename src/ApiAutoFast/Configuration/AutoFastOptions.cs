using NSwag.Generation.AspNetCore;

namespace ApiAutoFast;

public class AutoFastOptions
{
    public Type? AssemblyType { get; set; }
    public Action<AspNetCoreOpenApiDocumentGeneratorSettings>? OpenApiDocumentGeneratorSettings { get; set; }
}
