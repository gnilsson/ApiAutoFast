//HintName: PostModifyCommand.g.cs

 #nullable enable

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server.Database;

public class PostModifyCommand
{
    public string Id { get; set; }
    public int LikeCount { get; set; }
    public string Blog { get; set; }
    public string Tit { get; set; }
    public string PublicationDateTime { get; set; }
    public string Description { get; set; }
    public ApiAutoFast.Sample.Server.Database.EPostType PostType { get; set; }
}
