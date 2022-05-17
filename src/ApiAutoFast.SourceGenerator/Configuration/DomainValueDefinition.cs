namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct DomainValueDefinition
{
    internal DomainValueDefinition(string requestType, string entityType, string responseType, string name, string typeName, PropertyRelation propertyRelation)
    {
        RequestType = requestType;
        EntityType = entityType;
        ResponseType = responseType;
        Name = name;
        TypeName = typeName;
        PropertyRelation = propertyRelation;
    }

    internal readonly string RequestType { get; }
    internal readonly string EntityType { get; }
    internal readonly string ResponseType { get; }
    internal readonly string Name { get; }
    internal readonly string TypeName { get; }
    internal readonly PropertyRelation PropertyRelation { get; }
}
