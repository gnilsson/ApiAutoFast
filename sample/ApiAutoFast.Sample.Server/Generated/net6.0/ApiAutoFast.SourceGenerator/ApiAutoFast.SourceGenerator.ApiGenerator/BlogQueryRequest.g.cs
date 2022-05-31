
 #nullable enable

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server;

public class BlogQueryRequest
{
    public string? CreatedDateTime { get; set; }
    public string? ModifiedDateTime { get; set; }
    public string? Title { get; set; }
}
