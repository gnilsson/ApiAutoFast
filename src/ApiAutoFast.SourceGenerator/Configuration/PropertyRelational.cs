using ApiAutoFast.SourceGenerator.Configuration.Enums;

namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct PropertyRelational
{
    internal PropertyRelational(string foreignEntityName, string foreigEntityProperty, RelationalType relationalType)
    {
        ForeignEntityName = foreignEntityName;
        ForeigEntityProperty = foreigEntityProperty;
        RelationalType = relationalType;
    }

    internal readonly string ForeignEntityName { get; }
    internal readonly string ForeigEntityProperty { get; }
    internal readonly RelationalType RelationalType { get; }
}
