
using ApiAutoFast;

namespace ApiAutoFast.Sample.Server.Database;

public class PostCreateCommand
{
    public ApiAutoFast.Sample.Server.Database.Title Title { get; set; }
    public ApiAutoFast.Sample.Server.Database.ComplexDate ComplexDate { get; set; }
}
