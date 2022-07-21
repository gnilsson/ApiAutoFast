//HintName: BlogResponse2.g.cs

#nullable enable

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server;

public partial class BlogResponse
{
    public string Id { get; set; } = default!;
    public string CreatedDateTime { get; set; } = default!;
    public string ModifiedDateTime { get; set; } = default!;
    public string? Title { get; set; }
    public ICollection<PostResponseSimplified> Posts { get; set; }
    public ICollection<AuthorResponseSimplified> Authors { get; set; }
}
