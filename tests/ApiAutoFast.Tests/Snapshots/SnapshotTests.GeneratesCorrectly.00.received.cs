//HintName: Author.g.cs

#nullable enable

using ApiAutoFast;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ApiAutoFast.Sample.Server;

[Index(nameof(CreatedDateTime), nameof(Id))]
public class Author : IEntity<Identifier>
{
    public Author()
    {
        this.Blogs = new HashSet<Blog>();
    }

    public Identifier Id { get; set; } = default!;
    public DateTime CreatedDateTime { get; set; } = default!;
    public DateTime ModifiedDateTime { get; set; } = default!;
    public FirstName? FirstName { get; set; }
    public LastName? LastName { get; set; }
    public ICollection<Blog> Blogs { get; set; }
}
