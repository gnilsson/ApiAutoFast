namespace ApiAutoFast;


[AttributeUsage(AttributeTargets.Property)]
public class CreateCommandAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Property)]
public class ModifyCommandAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Property)]
public class QueryRequestAttribute : Attribute { }
