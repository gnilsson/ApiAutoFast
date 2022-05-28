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
        IdPropertyName = string.Empty;
    }

    internal PropertyRelation(string foreignEntityName, string foreigEntityProperty, RelationalType relationalType)
    {
        ForeignEntityName = foreignEntityName;
        ForeigEntityProperty = foreigEntityProperty;
        Type = relationalType;
        IdPropertyName = relationalType switch
        {
            RelationalType.ToMany => $"{ForeignEntityName}Ids",
            RelationalType.ToOne => $"{ForeignEntityName}Id",
            _ => string.Empty
        };
    }

    internal readonly string ForeignEntityName { get; }
    internal readonly string ForeigEntityProperty { get; }
    internal readonly RelationalType Type { get; }
    internal readonly string IdPropertyName { get; }
}
