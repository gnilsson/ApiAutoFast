using ApiAutoFast.SourceGenerator.Configuration.Enums;

namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct AutoFastEndpointsAttributeArguments
{
    public AutoFastEndpointsAttributeArguments(string entityName, EndpointTargetType endpointTargetType)
    {
        EntityName = entityName;
        EndpointTargetType = endpointTargetType;
    }

    internal readonly string EntityName { get; }
    internal readonly EndpointTargetType EndpointTargetType { get; }
}
