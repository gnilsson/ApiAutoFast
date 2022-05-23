
 #nullable enable

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server;

public class PostCreateCommand
{
    public string Title { get; set; }
    public string PublicationDateTime { get; set; }
    public string Description { get; set; }
    public ApiAutoFast.Sample.Server.EPostType PostType { get; set; }
    public int LikeCount { get; set; }
    public string BlogId { get; set; }
}
