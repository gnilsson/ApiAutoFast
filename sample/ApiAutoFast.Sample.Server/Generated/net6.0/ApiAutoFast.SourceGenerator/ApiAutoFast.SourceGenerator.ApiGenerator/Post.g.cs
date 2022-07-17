
#nullable enable

using ApiAutoFast;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ApiAutoFast.Sample.Server;

[Index(nameof(CreatedDateTime), nameof(Id))]
public class Post : IEntity<Identifier>
{
    public Post()
    {
    }

    public Identifier Id { get; set; } = default!;
    public DateTime CreatedDateTime { get; set; } = default!;
    public DateTime ModifiedDateTime { get; set; } = default!;
    public Title? Title { get; set; }
    public PublicationDateTime? PublicationDateTime { get; set; }
    public Description? Description { get; set; }
    public PostType? PostType { get; set; }
    public LikeCount? LikeCount { get; set; }
    [Required]
    public Blog Blog { get; set; } = default!;
    [Required]
    public SequentialIdentifier BlogId { get; set; } = default!;
}
