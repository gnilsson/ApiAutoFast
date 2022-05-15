
using ApiAutoFast;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ApiAutoFast.Sample.Server.Database;

public class Post : IEntity
{
    public Post()
    {
    }

    public Identifier Id { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime ModifiedDateTime { get; set; }
    public ApiAutoFast.Sample.Server.Database.Title Title { get; set; }
    public ApiAutoFast.Sample.Server.Database.PublicationDateTime PublicationDateTime { get; set; }
    public ApiAutoFast.Sample.Server.Database.Description Description { get; set; }
    public ApiAutoFast.Sample.Server.Database.PostType PostType { get; set; }
    // note: how to solve this problem?
    //public PostResponse AdaptToResponse(string? nothing = null)
    //{
    //    return null!;
    //}
}

public partial class PostResponse { }

//public static partial class PostMapper
//{
//    public static PostResponse AdaptToResponse(this Post p1)
//    {
//        return null!;
//    }
//}
