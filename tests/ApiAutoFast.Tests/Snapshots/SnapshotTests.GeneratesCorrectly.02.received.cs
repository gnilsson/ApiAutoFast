//HintName: AuthorResponse2.g.cs

#nullable enable

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server;

public partial class AuthorResponse
{
    public string Id { get; set; } = default!;
    public string CreatedDateTime { get; set; } = default!;
    public string ModifiedDateTime { get; set; } = default!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public ICollection<BlogResponseSimplified> Blogs { get; set; }
}
