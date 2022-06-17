
 #nullable enable

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server;

public partial class PostQueryRequest
{
    public string? CreatedDateTime { get; set; }
    public string? ModifiedDateTime { get; set; }
    public string? Title { get; set; }
    public string? PublicationDateTime { get; set; }
    public string? Description { get; set; }
    public ApiAutoFast.Sample.Server.EPostType? PostType { get; set; }
    public int? LikeCount { get; set; }
    public string? BlogId { get; set; }
}
