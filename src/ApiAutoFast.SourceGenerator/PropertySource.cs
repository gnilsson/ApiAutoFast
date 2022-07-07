namespace ApiAutoFast.SourceGenerator;

internal readonly struct PropertySource
{
    internal readonly string Entity { get; }
    internal readonly string Request { get; }
    internal readonly string Command { get; }
    internal readonly string Id { get; }
    internal readonly string MetadataType { get; }

    public PropertySource(string entity, string request, string command, string id, string metadataType)
    {
        Entity = entity;
        Request = request;
        Command = command;
        Id = id;
        MetadataType = metadataType;
    }

    public PropertySource(string entity, string metadataType)
    {
        Entity = entity;
        MetadataType = metadataType;
        Request = string.Empty;
        Command = string.Empty;
        Id = string.Empty;
    }
}
