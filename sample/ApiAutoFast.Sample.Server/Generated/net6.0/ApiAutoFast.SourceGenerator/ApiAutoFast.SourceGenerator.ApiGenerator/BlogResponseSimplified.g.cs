
#nullable enable

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server;

public partial class BlogResponseSimplified
{
    public string Id { get; init; } = default!;
    public string CreatedDateTime { get; init; } = default!;
    public string ModifiedDateTime { get; init; } = default!;
    public string? Title { get; init; }
    public IEnumerable<string>? Posts { get; init; }
    public IEnumerable<string>? Authors { get; init; }
}
