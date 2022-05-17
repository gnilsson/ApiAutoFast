
using ApiAutoFast;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ApiAutoFast.Sample.Server.Database;

public class Blog : IEntity
{
    public Blog()
    {
        this.Posts = new HashSet<Post>();
    }

    public Identifier Id { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime ModifiedDateTime { get; set; }
    public string Title { get; set; }
    public System.Collections.Generic.ICollection<Post> Posts { get; set; }
}
