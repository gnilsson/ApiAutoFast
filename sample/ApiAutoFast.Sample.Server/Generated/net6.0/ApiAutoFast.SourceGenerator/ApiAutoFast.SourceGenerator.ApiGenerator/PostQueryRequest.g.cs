
using ApiAutoFast;

namespace ApiAutoFast.Sample.Server.Database;

public class PostQueryRequest
{
    public string? CreatedDateTime { get; set; }
    public string? ModifiedDateTime { get; set; }
    public string? Title { get; set; }
    public string? PublicationDateTime { get; set; }
    public string? Description { get; set; }
}
