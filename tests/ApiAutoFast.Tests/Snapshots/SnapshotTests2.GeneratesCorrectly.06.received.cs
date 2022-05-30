//HintName: Post.g.cs

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
    public LikeCount LikeCount { get; set; }
    public Identifier BlogId { get; set; }
    public Blog Blog { get; set; }
    public Title Title { get; set; }
    public PublicationDateTime PublicationDateTime { get; set; }
    public Description Description { get; set; }
    public PostType PostType { get; set; }
}
