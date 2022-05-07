//HintName: AuthorCreateCommand.g.cs

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server.Database;

public class AuthorCreateCommand
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public ProfessionCategory Profession { get; set; }
}
