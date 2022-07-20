
#nullable enable

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server;

public partial class BlogResponse2
{
    public string? Id { get; set; }
    public string? CreatedDateTime { get; set; }
    public string? ModifiedDateTime { get; set; }
    public string? Title { get; set; }
    public System.Collections.Generic.ICollection<PostResponseSimplified> Posts { get; set; }
    public System.Collections.Generic.ICollection<AuthorResponseSimplified> Authors { get; set; }
}
