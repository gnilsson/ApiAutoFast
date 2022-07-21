
//////using ApiAutoFast;
//////using System.ComponentModel.DataAnnotations;
//////using Microsoft.EntityFrameworkCore;
//////using System.ComponentModel.DataAnnotations.Schema;

//////namespace ApiAutoFast.Sample.Server;

//////[Index(nameof(CreatedDateTime), nameof(Id))]
//////public class Post2 : IEntity<Identifier>
//////{
//////    public Post2()
//////    {
//////    }

//////    public Identifier Id { get; set; } = default!;
//////    public DateTime CreatedDateTime { get; set; } = default!;
//////    public DateTime ModifiedDateTime { get; set; } = default!;
//////    public Title? Title { get; set; }
//////    public PublicationDateTime? PublicationDateTime { get; set; }
//////    public Description? Description { get; set; }
//////    public PostType? PostType { get; set; }
//////    public LikeCount? LikeCount { get; set; }

//////    [Required]
//////    public Blog Blog { get; set; } = default!;
//////    public SequentialIdentifier BlogId { get; set; } = default!;
//////}



//////[Index(nameof(CreatedDateTime), nameof(Id))]
//////public class Blog2 : IEntity<SequentialIdentifier>
//////{
//////    public Blog2()
//////    {
//////        this.Posts = new HashSet<Post>();
//////    }

//////    public SequentialIdentifier Id { get; set; } = default!;
//////    public DateTime CreatedDateTime { get; set; } = default!;
//////    public DateTime ModifiedDateTime { get; set; } = default!;
//////    public Title? Title { get; set; }
//////    public System.Collections.Generic.ICollection<Post> Posts { get; set; }
//////}



////#nullable enable

////using ApiAutoFast;
////using Microsoft.Extensions.Hosting;
////using System.Reflection.Metadata;

////namespace ApiAutoFast.Sample.Server;

////public static partial class PostMapper3
////{
////    public static PostResponse MapToResponse2(this Post p0)
////    {
////        if (p0 is null) return null!;

////        return new PostResponse
////        {
////            Id = p0.Id.ToString(),
////            CreatedDateTime = ((ITimestamp)p0).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH: mm"),
////            ModifiedDateTime = ((ITimestamp)p0).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
////            Title = p0.Title?.ToResponse(),
////            PublicationDateTime = p0.PublicationDateTime?.ToResponse(),
////            Description = p0.Description?.ToResponse(),
////            PostType = p0.PostType?.ToResponse(),
////            LikeCount = p0.LikeCount?.ToResponse(),
////            BlogId = p0.BlogId?.ToResponse(),
////            Blog = MapSimple01(p0.Blog),
////};
////    }

////    private static BlogResponseSimplified MapSimple01(Blog p1)
////    {
////        if (p1 is null) return null!;

////        return new BlogResponseSimplified
////        {
////            Id = p1.Id.ToString(),
////            CreatedDateTime = ((ITimestamp)p1).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH: mm"),
////            ModifiedDateTime = ((ITimestamp)p1).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
////            Title = p1.Title?.ToResponse(),
////            Posts = p1.Posts?.ToResponse(),
////            Authors = p1.Authors?.ToResponse(),
////        };
////    }

////    private void Test()
////    {
////        var f = new List<string>() { "hej" } as ICollection<string>;
////        ICollection<string> obj;

////        obj = f.Select(x => x) .Select(x => x.ToString());
////    }
////}


//#nullable enable

//using ApiAutoFast;
//using System.Linq;

//namespace ApiAutoFast.Sample.Server;

//public static partial class BlogMapper3
//{
//    public static BlogResponse MapToResponse(this Blog p0)
//    {
//        if (p0 is null) return null!;

//        return new BlogResponse
//        {
//            Id = p0.Id.ToString(),
//            CreatedDateTime = ((ITimestamp)p0).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH: mm"),
//            ModifiedDateTime = ((ITimestamp)p0).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
//            Title = p0.Title?.ToResponse(),
//            Posts = MapEnumerable0(p0.Posts),
//            //    Authors = MapSimple1(p0.Authors),
//        };
//    }

//    private static PostResponseSimplified MapSimple0(Post p1)
//    {
//        if (p1 is null) return null!;

//        return new PostResponseSimplified
//        {
//            Id = p1.Id.ToString(),
//            CreatedDateTime = p1.CreatedDateTime.ToString("dddd, dd MMMM yyyy HH: mm"),
//            ModifiedDateTime = p1.ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
//            Title = p1.Title?.ToResponse(),
//            PublicationDateTime = p1.PublicationDateTime?.ToResponse(),
//            Description = p1.Description?.ToResponse(),
//            PostType = p1.PostType?.ToResponse(),
//            LikeCount = p1.LikeCount?.ToResponse(),
//            Blog = p1.BlogId.ToString(),
//        };
//    }

//    private static IEnumerable<PostResponseSimplified> MapEnumerable0(IEnumerable<Post> ep1)
//    {
//        if (ep1 is null) yield break;

//        foreach (var p1 in ep1)
//        {
//            yield return new PostResponseSimplified
//            {
//                Id = p1.Id.ToString(),
//                CreatedDateTime = p1.CreatedDateTime.ToString("dddd, dd MMMM yyyy HH: mm"),
//                ModifiedDateTime = p1.ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
//                Title = p1.Title?.ToResponse(),
//                PublicationDateTime = p1.PublicationDateTime?.ToResponse(),
//                Description = p1.Description?.ToResponse(),
//                PostType = p1.PostType?.ToResponse(),
//                LikeCount = p1.LikeCount?.ToResponse(),
//                Blog = p1.BlogId.ToString(),
//            };

//        }
//    }

//    //private static AuthorResponseSimplified MapSimple1(Author p2)
//    //{
//    //    if (p2 is null) return null!;

//    //    return new AuthorResponseSimplified
//    //    {
//    //        Id = p2.Id.ToString(),
//    //        CreatedDateTime = p2.CreatedDateTime.ToString("dddd, dd MMMM yyyy HH: mm"),
//    //        ModifiedDateTime = p2.ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
//    //        FirstName = p2.FirstName?.ToResponse(),
//    //        LastName = p2.LastName?.ToResponse(),
//    //        Blogs = p2.Blogs?.Select(x => x.Id),
//    //    };
//    //}

//}

