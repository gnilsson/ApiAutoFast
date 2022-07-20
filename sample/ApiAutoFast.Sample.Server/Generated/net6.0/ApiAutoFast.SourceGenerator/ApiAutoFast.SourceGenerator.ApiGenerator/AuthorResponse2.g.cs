
#nullable enable

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server;

public partial class AuthorResponse2
{
    public string? Id { get; set; }
    public string? CreatedDateTime { get; set; }
    public string? ModifiedDateTime { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public System.Collections.Generic.ICollection<BlogResponseSimplified> Blogs { get; set; }
}
