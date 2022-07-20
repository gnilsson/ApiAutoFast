
 #nullable enable

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server;

public partial class AuthorQueryRequest
{
    public string? CreatedDateTime { get; set; }
    public string? ModifiedDateTime { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
