//HintName: AuthorResponseSimplified.g.cs

#nullable enable

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server;

public partial class AuthorResponseSimplified
{
    public string Id { get; set; } = default!;
    public string CreatedDateTime { get; set; } = default!;
    public string ModifiedDateTime { get; set; } = default!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public IEnumerable<string> Blogs { get; set; }
}
