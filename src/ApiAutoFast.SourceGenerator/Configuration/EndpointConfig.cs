using ApiAutoFast.SourceGenerator.Configuration.Enums;

namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct EndpointConfig
{
    internal EndpointConfig(EntityConfig entityConfig, RequestEndpointPair requestEndpointPair)
    {
        var endpointTarget = requestEndpointPair.EndpointTarget.ToString();
        Name = $"{endpointTarget}{entityConfig.BaseName}Endpoint";
        EntityName = entityConfig.BaseName;
        MappingProfile = entityConfig.MappingProfile;
        Route = requestEndpointPair.EndpointTarget switch
        {
            EndpointTargetType.GetById
            or EndpointTargetType.Update
            or EndpointTargetType.Delete => $"/{entityConfig.BaseName.ToLower()}s/{{id}}",
            _ => $"/{entityConfig.BaseName.ToLower()}s",
        };
        Response = requestEndpointPair.EndpointTarget switch
        {
            EndpointTargetType.Get => $"Paginated<{entityConfig.Response}>",
            _ => entityConfig.Response,
        };
        Request = $"{entityConfig.BaseName}{requestEndpointPair.RequestModel}";
        RequestEndpointPair = requestEndpointPair;
    }

    internal readonly string Name { get; }
    internal readonly string Response { get; }
    internal readonly string Request { get; }
    internal readonly string EntityName { get; }
    internal readonly string Route { get; }
    internal readonly string MappingProfile { get; }
    internal readonly RequestEndpointPair RequestEndpointPair { get; }
}
