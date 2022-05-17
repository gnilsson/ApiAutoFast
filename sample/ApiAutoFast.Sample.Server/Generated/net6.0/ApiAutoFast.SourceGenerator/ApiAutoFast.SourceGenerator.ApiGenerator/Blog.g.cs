
using ApiAutoFast;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ApiAutoFast.Sample.Server.Database;

public class Blog : IEntity
{
    public Blog()
    {
        this.Posts = PostsRelation.From(new HashSet<Post>());
    }

    public Identifier Id { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime ModifiedDateTime { get; set; }
    public Title Title { get; set; }
    public PostsRelation Posts { get; set; }
}
