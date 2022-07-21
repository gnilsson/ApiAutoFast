//HintName: BlogMapper2.g.cs

#nullable enable

using ApiAutoFast;
using System.Linq;

namespace ApiAutoFast.Sample.Server;

public static partial class BlogMapper2
{
    public static BlogResponse MapToResponse(this Blog p0)
    {
        if(p0 is null) return null!;

        return new BlogResponse
        {
            Id = p0.Id.ToString(),
            CreatedDateTime = ((ITimestamp)p0).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH: mm"),
            ModifiedDateTime = ((ITimestamp)p0).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
            Title = p0.Title?.ToResponse(),
            Posts = MapSimple0(p0.Posts),
            Authors = MapSimple1(p0.Authors),
        };
    }

    private static PostResponseSimplified MapSimple0(Post p1)
    {
        if(p1 is null) return null!;

        return new PostResponseSimplified
        {
            Id = p1.Id.ToString(),
            CreatedDateTime = p1.CreatedDateTime.ToString("dddd, dd MMMM yyyy HH: mm"),
            ModifiedDateTime = p1.ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
            LikeCount = p1.LikeCount?.ToResponse(),
            BlogId = p1.SequentialIdentifier?.ToResponse(),
            Blog = p1.BlogRelation?.ToResponse(),
            Tit = p1.Title?.ToResponse(),
            PublicationDateTime = p1.PublicationDateTime?.ToResponse(),
            Description = p1.Description?.ToResponse(),
            PostType = p1.PostType?.ToResponse(),
        };
    }
    private static AuthorResponseSimplified MapSimple1(Author p2)
    {
        if(p2 is null) return null!;

        return new AuthorResponseSimplified
        {
            Id = p2.Id.ToString(),
            CreatedDateTime = p2.CreatedDateTime.ToString("dddd, dd MMMM yyyy HH: mm"),
            ModifiedDateTime = p2.ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
            FirstName = p2.FirstName?.ToResponse(),
            LastName = p2.LastName?.ToResponse(),
            Blogs = p2.BlogsRelation?.Select(x => x.Id),
        };
    }
}
