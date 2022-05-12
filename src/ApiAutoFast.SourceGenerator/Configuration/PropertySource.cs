namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct PropertySource
{
    internal PropertySource(string entityModel, string? requestModel = null)
    {
        EntityModel = entityModel;
        _requestModel = requestModel;
    }

    internal readonly string? _requestModel = null;
    internal readonly string EntityModel { get; }
    internal readonly string RequestModel => _requestModel is not null ? _requestModel : EntityModel;
}
