//HintName: PostQueryRequest.g.cs

 #nullable enable

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server.Database;

public class PostQueryRequest
{
    public string? CreatedDateTime { get; set; }
    public string? ModifiedDateTime { get; set; }
    public int? LikeCount { get; set; }
    public string? Blog { get; set; }
    public string? Tit { get; set; }
    public string? PublicationDateTime { get; set; }
    public string? Description { get; set; }
    public ApiAutoFast.Sample.Server.Database.EPostType? PostType { get; set; }
}
