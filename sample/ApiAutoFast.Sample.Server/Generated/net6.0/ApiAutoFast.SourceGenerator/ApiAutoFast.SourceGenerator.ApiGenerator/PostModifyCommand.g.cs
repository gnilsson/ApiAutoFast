
using ApiAutoFast;

namespace ApiAutoFast.Sample.Server;

public class PostModifyCommand
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string PublicationDateTime { get; set; }
    public string Description { get; set; }
    public ApiAutoFast.Sample.Server.EPostType PostType { get; set; }
    public int LikeCount { get; set; }
    public string BlogId { get; set; }
}
