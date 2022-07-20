
 #nullable enable

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server;

public partial class AuthorCreateCommand
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
