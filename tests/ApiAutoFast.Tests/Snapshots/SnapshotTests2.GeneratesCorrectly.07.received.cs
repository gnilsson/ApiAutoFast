//HintName: PostCreateCommand.g.cs

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server.Database;

public class PostCreateCommand
{
    public int LikeCount { get; set; }
    public string Title { get; set; }
    public string PublicationDateTime { get; set; }
    public string Description { get; set; }
    public ApiAutoFast.Sample.Server.Database.EPostType PostType { get; set; }
}
