//HintName: PostQueryRequest.g.cs

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server.Database;

public class PostQueryRequest
{
    public string? CreatedDateTime { get; set; }
    public string? ModifiedDateTime { get; set; }
    public Title Title { get; set; }
}
