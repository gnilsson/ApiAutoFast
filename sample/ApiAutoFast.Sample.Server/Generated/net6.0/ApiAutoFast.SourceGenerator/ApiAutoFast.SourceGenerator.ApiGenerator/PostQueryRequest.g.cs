
using ApiAutoFast;

namespace ApiAutoFast.Sample.Server.Database;

public class PostQueryRequest
{
    public string? CreatedDateTime { get; set; }
    public string? ModifiedDateTime { get; set; }
    public ApiAutoFast.Sample.Server.Database.Title Title { get; set; }
    public ApiAutoFast.Sample.Server.Database.ComplexDate ComplexDate { get; set; }
}
