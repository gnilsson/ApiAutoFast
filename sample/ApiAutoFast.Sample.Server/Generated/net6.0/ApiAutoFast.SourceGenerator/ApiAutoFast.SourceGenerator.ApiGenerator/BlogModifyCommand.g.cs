
using ApiAutoFast;

namespace ApiAutoFast.Sample.Server.Database;

public class BlogModifyCommand
{
    public string Id { get; set; }
    public string? Title { get; set; }
    public string? AuthorId { get; set; }
}
