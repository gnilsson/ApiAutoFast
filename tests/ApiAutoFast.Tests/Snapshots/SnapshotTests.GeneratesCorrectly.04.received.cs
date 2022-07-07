//HintName: BlogCreateCommand.g.cs

 #nullable enable

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server.Database;

public partial class BlogCreateCommand
{
    public string Title { get; set; }
    public int LikeCount { get; set; }
}
