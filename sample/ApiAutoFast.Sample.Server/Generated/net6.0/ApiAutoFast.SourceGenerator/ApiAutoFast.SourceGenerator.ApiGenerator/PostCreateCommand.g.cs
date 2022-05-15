
using ApiAutoFast;

namespace ApiAutoFast.Sample.Server.Database;

public class PostCreateCommand
{
    public string? Title { get; set; }
    public string? PublicationDateTime { get; set; }
    public string? Description { get; set; }
    public string? PostType { get; set; }
}
