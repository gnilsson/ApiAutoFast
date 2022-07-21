
#nullable enable

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server;

public partial class AuthorResponse
{
    public string Id { get; init; } = default!;
    public string CreatedDateTime { get; init; } = default!;
    public string ModifiedDateTime { get; init; } = default!;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public System.Collections.Generic.IEnumerable<BlogResponseSimplified> Blogs { get; init; }
}
