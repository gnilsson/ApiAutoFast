
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

    public Identifier Id { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime ModifiedDateTime { get; set; }
    public Title Title { get; set; }
    public PublicationDateTime PublicationDateTime { get; set; }
    public Description Description { get; set; }
    public PostType PostType { get; set; }
    public LikeCount LikeCount { get; set; }
    public Blog Blog { get; set; }
    public SequentialIdentifier BlogId { get; set; }
}
