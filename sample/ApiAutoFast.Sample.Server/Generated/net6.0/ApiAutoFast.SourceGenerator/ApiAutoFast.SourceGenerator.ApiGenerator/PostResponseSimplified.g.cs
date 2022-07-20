
#nullable enable

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server;

public partial class PostResponseSimplified
{
    public string? Id { get; set; }
    public string? CreatedDateTime { get; set; }
    public string? ModifiedDateTime { get; set; }
    public string? Title { get; set; }
    public string? PublicationDateTime { get; set; }
    public string? Description { get; set; }
    public string? PostType { get; set; }
    public int? LikeCount { get; set; }
    public BlogResponseSimplified Blog { get; set; }
}
