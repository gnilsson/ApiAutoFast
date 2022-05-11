//HintName: AuthorCreateCommand.g.cs

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server.Database;

public class AuthorCreateCommand
{
    public string? LastName { get; set; }
    public ApiAutoFast.Sample.Server.Database.ProfessionCategory? Profession { get; set; }
}
