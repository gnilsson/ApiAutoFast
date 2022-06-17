
 #nullable enable

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server;

public partial class BlogCreateCommand
{
    public int LikeCount { get; set; }
    public string Title { get; set; }
}
