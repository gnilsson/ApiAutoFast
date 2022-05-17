using ApiAutoFast.SourceGenerator.Configuration.Enums;

namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct PropertyRelation
{
    public static PropertyRelation None = new(RelationalType.None);

    private PropertyRelation(RelationalType relationalType)
    {
        RelationalType = relationalType;
        ForeignEntityName = string.Empty;
        ForeigEntityProperty = string.Empty;
    }

    internal PropertyRelation(string foreignEntityName, string foreigEntityProperty, RelationalType relationalType)
    {
        ForeignEntityName = foreignEntityName;
        ForeigEntityProperty = foreigEntityProperty;
        RelationalType = relationalType;
    }

    internal readonly string ForeignEntityName { get; }
    internal readonly string ForeigEntityProperty { get; }
    internal readonly RelationalType RelationalType { get; }
}
