
//using ApiAutoFast;
//using System.ComponentModel.DataAnnotations;
//using Microsoft.EntityFrameworkCore;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace ApiAutoFast.Sample.Server;

//[Index(nameof(CreatedDateTime), nameof(Id))]
//public class Post2 : IEntity<Identifier>
//{
//    public Post2()
//    {
//    }

//    public Identifier Id { get; set; } = default!;
//    public DateTime CreatedDateTime { get; set; } = default!;
//    public DateTime ModifiedDateTime { get; set; } = default!;
//    public Title? Title { get; set; }
//    public PublicationDateTime? PublicationDateTime { get; set; }
//    public Description? Description { get; set; }
//    public PostType? PostType { get; set; }
//    public LikeCount? LikeCount { get; set; }

//    [Required]
//    public Blog Blog { get; set; } = default!;
//    public SequentialIdentifier BlogId { get; set; } = default!;
//}



//[Index(nameof(CreatedDateTime), nameof(Id))]
//public class Blog2 : IEntity<SequentialIdentifier>
//{
//    public Blog2()
//    {
//        this.Posts = new HashSet<Post>();
//    }

//    public SequentialIdentifier Id { get; set; } = default!;
//    public DateTime CreatedDateTime { get; set; } = default!;
//    public DateTime ModifiedDateTime { get; set; } = default!;
//    public Title? Title { get; set; }
//    public System.Collections.Generic.ICollection<Post> Posts { get; set; }
//}
