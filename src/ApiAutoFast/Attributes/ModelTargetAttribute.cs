using FastEndpoints;

namespace ApiAutoFast;


[AttributeUsage(AttributeTargets.Property)]
public class CreateCommandAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Property)]
public class ModifyCommandAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Property)]
public class QueryRequestAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Property)]
public class ForeignKeyOnlyAttribute : Attribute { }

//[AttributeUsage(AttributeTargets.Property)]
//public class ExcludeRequestModelAttribute : Attribute
//{
//    public ExcludeRequestModelAttribute(RequestModelTarget includeRequestModelTarget = RequestModelTarget.None)
//    {
//        IncludeRequestModelTarget = includeRequestModelTarget;
//    }

//    public RequestModelTarget IncludeRequestModelTarget { get; }
//}

//[Flags]
//public enum RequestModelTarget
//{
//    None = 0,
//    CreateCommand = 1,
//    ModifyCommand = 2,
//    QueryRequest = 4,
//    GetByIdRequest = 8,
//    DeleteCommand = 16,
//}


///// <summary>
///// Marker attribute for source generator.
///// <param name="entityName">Name of the entity to generate, will default to this class name and remove "Config"</param>
///// </summary>
//[System.AttributeUsage(System.AttributeTargets.Class)]
//internal class AutoFastEndpointsAttribute : System.Attribute
//{
//    internal AutoFastEndpointsAttribute(string? entityName = null)
//    {
//        EntityName = entityName;
//    }

//    public string? EntityName { get; }
//}
