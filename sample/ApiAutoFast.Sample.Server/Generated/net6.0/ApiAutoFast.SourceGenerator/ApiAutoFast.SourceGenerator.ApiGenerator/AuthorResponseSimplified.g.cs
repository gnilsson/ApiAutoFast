
#nullable enable

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server;

public partial class AuthorResponseSimplified
{
    public string Id { get; init; } = default!;
    public string CreatedDateTime { get; init; } = default!;
    public string ModifiedDateTime { get; init; } = default!;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public IEnumerable<string> Blogs { get; init; }
}
