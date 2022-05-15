//HintName: AutoFastEndpointsAttribute.g.cs

using System;

namespace ApiAutoFast;

/// <summary>
/// Marker attribute for source generator.
/// <param name="entityName">Name of the entity to generate, will default to this class name and remove "Config"</param>
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
internal class AutoFastEndpointsAttribute : Attribute
{
    internal AutoFastEndpointsAttribute(string? entityName = null, EndpointTargetType includeEndpointTarget = EndpointTargetType.All)
    {
        EntityName = entityName;
        IncludeEndpointTarget = includeEndpointTarget;
    }

    public string? EntityName { get; }
    public EndpointTargetType IncludeEndpointTarget { get; }
}
