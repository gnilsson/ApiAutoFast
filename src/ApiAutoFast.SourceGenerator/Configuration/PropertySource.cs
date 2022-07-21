namespace ApiAutoFast.SourceGenerator.Configuration;

//internal readonly struct PropertySource
//{
//    internal readonly string Entity { get; }
//    internal readonly string Request { get; }
//    internal readonly string Command { get; }
//    internal readonly string MetadataType { get; }
//    internal readonly string Response { get; }
//    internal readonly string Id { get; }
//    internal readonly string ResponseId { get; }

//    public PropertySource(string entity, string request, string command, string metadataType, string response, string id, string responseId)
//    {
//        Entity = entity;
//        Request = request;
//        Command = command;
//        MetadataType = metadataType;
//        Response = response;
//        Id = id;
//        ResponseId = responseId;
//    }

//    public PropertySource(string entity, string metadataType, string response)
//    {
//        Entity = entity;
//        MetadataType = metadataType;
//        Request = string.Empty;
//        Command = string.Empty;
//        Response = response;
//        Id = string.Empty;
//        ResponseId = string.Empty;
//    }
//}

internal class PropertySource
{
    internal string? Entity { get; set; }
    internal string? Request { get; set; }
    internal string? Command { get; set; }
    internal string? MetadataType { get; set; }
    internal string? Response { get; set; }
    internal string? Id { get; set; }
    internal string? ResponseId { get; set; }
}
