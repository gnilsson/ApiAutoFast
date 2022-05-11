//HintName: AuthorDeleteCommand.g.cs

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server.Database;

public class AuthorDeleteCommand
{
    public string Id { get; set; }
    public string? LastName { get; set; }
}
