//HintName: BlogQueryRequest.g.cs

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server.Database;

public class BlogQueryRequest
{
    public string? CreatedDateTime { get; set; }
    public string? ModifiedDateTime { get; set; }
    public string Title { get; set; }
    public Identifier AuthorId { get; set; }
}
