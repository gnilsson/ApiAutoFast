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
