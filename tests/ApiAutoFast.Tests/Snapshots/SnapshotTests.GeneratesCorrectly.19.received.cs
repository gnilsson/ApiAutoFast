//HintName: PostResponseSimplified.g.cs

#nullable enable

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server;

public partial class PostResponseSimplified
{
    public string Id { get; set; } = default!;
    public string CreatedDateTime { get; set; } = default!;
    public string ModifiedDateTime { get; set; } = default!;
    public int? LikeCount { get; set; }
    public string Blog { get; set; }
    public string? Tit { get; set; }
    public string? PublicationDateTime { get; set; }
    public string? Description { get; set; }
    public string? PostType { get; set; }
}
