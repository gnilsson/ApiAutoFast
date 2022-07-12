using ApiAutoFast.SourceGenerator.Configuration.Enums;

namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct AutoFastEntityAttributeArguments
{
    public AutoFastEntityAttributeArguments(string entityName, EndpointTargetType endpointTargetType, string idType)
    {
        EntityName = entityName;
        EndpointTargetType = endpointTargetType;
        IdType = idType;
    }

    internal readonly string EntityName { get; }
    internal readonly EndpointTargetType EndpointTargetType { get; }
    internal readonly string IdType { get; }
}
