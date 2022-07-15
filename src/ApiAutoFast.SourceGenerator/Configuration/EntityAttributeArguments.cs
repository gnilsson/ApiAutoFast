using ApiAutoFast.SourceGenerator.Configuration.Enums;

namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct EntityAttributeArguments
{
    public EntityAttributeArguments(string entityName, EndpointTargetType endpointTargetType, string idType)
    {
        EntityName = entityName;
        EndpointTargetType = endpointTargetType;
        IdType = idType;
    }

    internal readonly string EntityName { get; }
    internal readonly EndpointTargetType EndpointTargetType { get; }
    internal readonly string IdType { get; }
}
