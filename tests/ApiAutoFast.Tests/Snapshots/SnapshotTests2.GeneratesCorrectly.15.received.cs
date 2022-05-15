//HintName: PostModifyCommand.g.cs

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server.Database;

public class PostModifyCommand
{
    public string Id { get; set; }
    public string? Title { get; set; }
    public string? PublicationDateTime { get; set; }
    public string? Description { get; set; }
    public string? PostType { get; set; }
}
