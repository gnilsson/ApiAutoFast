
#nullable enable

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server;

public partial class BlogResponse
{
    public string Id { get; init; } = default!;
    public string CreatedDateTime { get; init; } = default!;
    public string ModifiedDateTime { get; init; } = default!;
    public string? Title { get; init; }
    public System.Collections.Generic.IEnumerable<PostResponseSimplified> Posts { get; init; }
    public System.Collections.Generic.IEnumerable<AuthorResponseSimplified> Authors { get; init; }
}
