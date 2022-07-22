
 #nullable enable

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server;

public partial class BlogModifyCommand
{
    public string Id { get; set; } = default!;
    public string? Title { get; set; }
    public IEnumerable<string>? Posts { get; set; }
    public IEnumerable<string>? Authors { get; set; }
}
