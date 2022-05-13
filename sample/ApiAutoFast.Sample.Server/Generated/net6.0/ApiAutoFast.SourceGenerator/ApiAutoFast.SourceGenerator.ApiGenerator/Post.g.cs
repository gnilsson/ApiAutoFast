
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
    public ApiAutoFast.Sample.Server.Database.ComplexDate ComplexDate { get; set; }
    public ApiAutoFast.ComplexString Description { get; set; }
}

public partial class PostResponse { }
