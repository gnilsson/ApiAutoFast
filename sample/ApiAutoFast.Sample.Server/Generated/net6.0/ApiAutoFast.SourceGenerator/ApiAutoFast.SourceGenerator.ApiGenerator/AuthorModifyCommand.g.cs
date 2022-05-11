
using ApiAutoFast;

namespace ApiAutoFast.Sample.Server.Database;

public class AuthorModifyCommand
{
    public string Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public ApiAutoFast.Sample.Server.Database.ProfessionCategory? Profession { get; set; }
}
