
 #nullable enable

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server;

public partial class BlogCreateCommand
{
    public string? Title { get; set; }
    public IEnumerable<string>? Posts { get; set; }
    public IEnumerable<string>? Authors { get; set; }
}
