
using ApiAutoFast;

namespace ApiAutoFast.Sample.Server.Database;

public class BlogCreateCommand
{
    public string Title { get; set; }
    public ApiAutoFast.Identifier AuthorId { get; set; }
}
