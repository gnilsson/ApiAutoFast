
using ApiAutoFast;

namespace ApiAutoFast.Sample.Server.Database;

public class PostModifyCommand
{
    public string Id { get; set; }
    public ApiAutoFast.Sample.Server.Database.Title Title { get; set; }
    public ApiAutoFast.Sample.Server.Database.PublicationDateTime PublicationDateTime { get; set; }
    public ApiAutoFast.Sample.Server.Database.Description Description { get; set; }
}
