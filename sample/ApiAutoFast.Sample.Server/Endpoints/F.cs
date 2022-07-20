
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



//#nullable enable

//using ApiAutoFast;

//namespace ApiAutoFast.Sample.Server;

//public static partial class PostMapper2
//{
//    public static PostResponse2 AdaptToResponse(this Post p0)
//    {
//        if (p0 is null) return null!;

//        return new PostResponse2
//        {
//            Id = p0.Id.ToString(),
//            CreatedDateTime = ((ITimestamp)p0).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH: mm"),
//            ModifiedDateTime = ((ITimestamp)p0).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
//            Title = p0.Title?.ToResponse(),
//            PublicationDateTime = p0.PublicationDateTime?.ToResponse(),
//            Description = p0.Description?.ToResponse(),
//            PostType = p0.PostType?.ToResponse(),
//            LikeCount = p0.LikeCount?.ToResponse(),
//            BlogId = p0.BlogId.ToString(),
//        };
//    }
//}

