
#nullable enable

using ApiAutoFast;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ApiAutoFast.Sample.Server;

[Index(nameof(CreatedDateTime), nameof(Id))]
public class Blog : IEntity<SequentialIdentifier>
{
    public Blog()
    {
        this.Posts = new HashSet<Post>();
        this.Authors = new HashSet<Author>();
    }

    public SequentialIdentifier Id { get; set; } = default!;
    public DateTime CreatedDateTime { get; set; } = default!;
    public DateTime ModifiedDateTime { get; set; } = default!;
    public Title? Title { get; set; }
    public System.Collections.Generic.ICollection<Post> Posts { get; set; }
    public System.Collections.Generic.ICollection<Author> Authors { get; set; }
}
