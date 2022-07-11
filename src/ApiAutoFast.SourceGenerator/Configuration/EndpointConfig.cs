using ApiAutoFast.SourceGenerator.Configuration.Enums;
using System.Collections.Immutable;

namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct EndpointConfig
{
    internal EndpointConfig(
        string endpoint,
        EntityConfig entityConfig,
        RequestEndpointPair requestEndpointPair,
        bool isTargetForGeneration,
        ImmutableArray<string> relationalNavigationNames,
        ImmutableArray<string> stringEntityProperties,
        string contextName)
    {
        Endpoint = endpoint;
        Entity = entityConfig.BaseName;
        MappingProfile = entityConfig.MappingProfile;
        Route = requestEndpointPair.EndpointTarget switch
        {
            EndpointTargetType.GetById
            or EndpointTargetType.Update
            or EndpointTargetType.Delete => $"/{entityConfig.BaseName.ToLower()}s/{{id}}",
            _ => $"/{entityConfig.BaseName.ToLower()}s",
        };

        Response = entityConfig.Response;
        Request = $"{entityConfig.BaseName}{requestEndpointPair.RequestModel}";
        RequestEndpointPair = requestEndpointPair;
        IsTargetForGeneration = isTargetForGeneration;
        RelationalNavigationNames = relationalNavigationNames;
        StringEntityProperties = stringEntityProperties;
        ContextName = contextName;
    }

    internal readonly string Endpoint { get; }
    internal readonly string Response { get; }
    internal readonly string Request { get; }
    internal readonly string Entity { get; }
    internal readonly string Route { get; }
    internal readonly string MappingProfile { get; }
    internal readonly RequestEndpointPair RequestEndpointPair { get; }
    internal readonly bool IsTargetForGeneration { get; }
    internal readonly ImmutableArray<string> RelationalNavigationNames { get; }
    internal readonly ImmutableArray<string> StringEntityProperties { get; }
    internal readonly string ContextName { get; }
}
