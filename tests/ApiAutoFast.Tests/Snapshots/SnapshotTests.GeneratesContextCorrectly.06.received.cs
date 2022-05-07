//HintName: AuthorQueryRequest.g.cs

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server.Database;

public class AuthorQueryRequest
{
    public string? CreatedDateTime { get; set; }
    public string? ModifiedDateTime { get; set; }
    public string? LastName { get; set; }
    public ProfessionCategory Profession { get; set; }
}
