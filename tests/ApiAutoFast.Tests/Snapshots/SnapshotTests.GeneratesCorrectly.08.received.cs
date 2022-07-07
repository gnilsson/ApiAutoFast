//HintName: BlogQueryRequest.g.cs

 #nullable enable

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server.Database;

public partial class BlogQueryRequest
{
    public string? CreatedDateTime { get; set; }
    public string? ModifiedDateTime { get; set; }
    public string? Title { get; set; }
}
