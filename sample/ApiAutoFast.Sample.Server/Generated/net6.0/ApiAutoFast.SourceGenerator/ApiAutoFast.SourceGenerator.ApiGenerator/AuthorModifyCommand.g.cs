
 #nullable enable

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server;

public partial class AuthorModifyCommand
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
