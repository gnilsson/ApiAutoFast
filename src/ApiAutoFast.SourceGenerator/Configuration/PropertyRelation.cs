using ApiAutoFast.SourceGenerator.Configuration.Enums;

namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct PropertyRelation
{
    internal static PropertyRelation None = new(RelationalType.None);

    private PropertyRelation(RelationalType relationalType)
    {
        Type = relationalType;
        ForeignEntityName = string.Empty;
        ForeigEntityProperty = string.Empty;
    }

    internal PropertyRelation(string foreignEntityName, string foreigEntityProperty, RelationalType relationalType)
    {
        ForeignEntityName = foreignEntityName;
        ForeigEntityProperty = foreigEntityProperty;
        Type = relationalType;
    }

    internal readonly string ForeignEntityName { get; }
    internal readonly string ForeigEntityProperty { get; }
    internal readonly RelationalType Type { get; }
}
