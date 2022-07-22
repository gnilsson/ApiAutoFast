
#nullable enable

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server;

public partial class PostResponseSimplified
{
    public string Id { get; init; } = default!;
    public string CreatedDateTime { get; init; } = default!;
    public string ModifiedDateTime { get; init; } = default!;
    public string? Title { get; init; }
    public string? PublicationDateTime { get; init; }
    public string? Description { get; init; }
    public string? PostType { get; init; }
    public int? LikeCount { get; init; }
    public string Blog { get; init; } = default!;
}
