namespace ApiAutoFast.SourceGenerator.Emitters;

internal static class EmbeddedSourceEmitter
{
    public const string AutoFastEntityAttribute = @"
#nullable enable

using System;

namespace ApiAutoFast;

/// <summary>
/// Marker attribute for source generator.
/// <param name=""entityName"">Name of the entity to generate, will default to current class name and remove ""Entity""</param>
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
internal class AutoFastEntityAttribute : Attribute
{
    internal AutoFastEntityAttribute(string? entityName = null, EndpointTargetType includeEndpointTarget = EndpointTargetType.All, IdType idType = IdType.Identifier)
    {
        EntityName = entityName;
        IncludeEndpointTarget = includeEndpointTarget;
        IdType = idType;
    }

    public string? EntityName { get; }
    public EndpointTargetType IncludeEndpointTarget { get; }
    public IdType IdType { get; }
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

    public const string AutoFastEndpointAttribute = @"
using System;

namespace ApiAutoFast;

[AttributeUsage(AttributeTargets.Class)]
internal class AutoFastEndpointAttribute : Attribute
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
[AttributeUsage(AttributeTargets.Class)]
public class ExcludeRequestModelAttribute : Attribute
{
    public ExcludeRequestModelAttribute(RequestModelTarget includeRequestModelTarget = RequestModelTarget.None)
    {
        IncludeRequestModelTarget = includeRequestModelTarget;
    }

    public RequestModelTarget IncludeRequestModelTarget { get; }
}
";

    //note: the usefulness of this attribute is highly questionable, partial commands seems better.
    public const string IncludeInCommandAttribute = @"
    using System;

    namespace ApiAutoFast;

    /// <summary>
    /// Attribute to include property in another entity command.
    /// <param name=""otherEntityType"">The other entity</param>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class IncludeInCommandAttribute : Attribute
    {
        public IncludeInCommandAttribute(params Type[] otherEntityTypes)
        {
            OtherEntityTypes = otherEntityTypes;
        }

        public Type[] OtherEntityTypes { get; }
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

    public const string IdTypeEnum = @"
namespace ApiAutoFast;

internal enum IdType
{
    Identifier,
    SequentialIdentifier,
}
";
}
