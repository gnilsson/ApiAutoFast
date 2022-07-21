//HintName: BlogResponseSimplified.g.cs

#nullable enable

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server;

public partial class BlogResponseSimplified
{
    public string Id { get; set; } = default!;
    public string CreatedDateTime { get; set; } = default!;
    public string ModifiedDateTime { get; set; } = default!;
    public string? Title { get; set; }
    public IEnumerable<string> Posts { get; set; }
    public IEnumerable<string> Authors { get; set; }
}
