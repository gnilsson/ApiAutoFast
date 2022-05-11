
using ApiAutoFast;

namespace ApiAutoFast.Sample.Server.Database;

public class BlogCreateCommand
{
    public string? Title { get; set; }
    public Author Author { get; set; }
    public string? AuthorId { get; set; }
}
