
using System;

namespace ApiAutoFast;

/// <summary>
/// Marker attribute for source generator.
/// <param name="entityName">Name of the entity to generate, will default to current class name and remove "Entity"</param>
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
internal class AutoFastEntityAttribute : Attribute
{
    internal AutoFastEntityAttribute(string? entityName = null, EndpointTargetType includeEndpointTarget = EndpointTargetType.All)
    {
        EntityName = entityName;
        IncludeEndpointTarget = includeEndpointTarget;
    }

    public string? EntityName { get; }
    public EndpointTargetType IncludeEndpointTarget { get; }
}
