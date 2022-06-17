//HintName: BlogModifyCommand.g.cs

 #nullable enable

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server.Database;

public partial class BlogModifyCommand
{
    public string Id { get; set; }
    public string Title { get; set; }
}
