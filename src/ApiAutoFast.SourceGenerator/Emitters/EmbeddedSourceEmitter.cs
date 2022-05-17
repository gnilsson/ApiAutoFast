namespace ApiAutoFast.SourceGenerator.Emitters;

internal static class EmbeddedSourceEmitter
{
    public const string AutoFastEndpointsAttribute = @"
using System;

namespace ApiAutoFast;

/// <summary>
/// Marker attribute for source generator.
/// <param name=""entityName"">Name of the entity to generate, will default to current class name and remove ""Entity""</param>
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
";

    public const string AutoFastContextAttribute = @"
using System;

namespace ApiAutoFast;

[AttributeUsage(AttributeTargets.Class)]
internal class AutoFastContextAttribute : Attribute
{
}
";

    public const string ExcludeRequestModelAttribute = @"
using System;

namespace ApiAutoFast;

/// <summary>
/// Attribute to exclude property from request model.
/// <param name=""includeRequestModelTarget"">If not applied, property is per default included in
/// RequestModelTarget.CreateCommand | RequestModelTarget.ModifyCommand | RequestModelTarget.QueryRequest</param>
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ExcludeRequestModelAttribute : Attribute
{
    public ExcludeRequestModelAttribute(RequestModelTarget includeRequestModelTarget = RequestModelTarget.None)
    {
        IncludeRequestModelTarget = includeRequestModelTarget;
    }

    public RequestModelTarget IncludeRequestModelTarget { get; }
}
";

    public const string RequestModelTargetEnum = @"
using System;

namespace ApiAutoFast;

[Flags]
public enum RequestModelTarget
{
    None = 0,
    CreateCommand = 1,
    ModifyCommand = 2,
    QueryRequest = 4,
    GetByIdRequest = 8,
    DeleteCommand = 16,
}
";

    public const string EndpointTargetEnum = @"
using System;

namespace ApiAutoFast;

[Flags]
internal enum EndpointTargetType
{
    None = 0,
    Get = 1,
    GetById = 2,
    Create = 4,
    Update = 8,
    Delete = 16,
    All = Get | Create | Update | GetById | Delete
}
";
}
