
#nullable enable

using ApiAutoFast;
using System.Linq;

namespace ApiAutoFast.Sample.Server;

public static partial class BlogMapper2
{
    public static BlogResponse MapToResponse(this Blog p)
    {
        if (p is null) return null!;

        return new BlogResponse
        {
            Id = p.Id.ToString(),
            CreatedDateTime = p.CreatedDateTime.ToString("dddd, dd MMMM yyyy HH: mm"),
            ModifiedDateTime = p.ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
            Title = p.Title?.ToResponse(),
            Posts = MapEnumerable0(p.Posts),
            Authors = MapEnumerable1(p.Authors),
        };
    }

    private static PostResponseSimplified MapEnumerable0(Post ep1)
    {
        if (ep1 is null) yield break;

        foreach (var p in ep1)
        {
            yield return new PostResponseSimplified
            {
                Id = p.Id.ToString(),
                CreatedDateTime = p.CreatedDateTime.ToString("dddd, dd MMMM yyyy HH: mm"),
                ModifiedDateTime = p.ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
                Title = ep1.Title?.ToResponse(),
                PublicationDateTime = ep1.PublicationDateTime?.ToResponse(),
                Description = ep1.Description?.ToResponse(),
                PostType = ep1.PostType?.ToResponse(),
                LikeCount = ep1.LikeCount?.ToResponse(),
                Blog = ep1.BlogId.ToString(),
            };
        }
    }

    private static AuthorResponseSimplified MapEnumerable1(Author ep2)
    {
        if (ep2 is null) yield break;

        foreach (var p in ep2)
        {
            yield return new AuthorResponseSimplified
            {
                Id = p.Id.ToString(),
                CreatedDateTime = p.CreatedDateTime.ToString("dddd, dd MMMM yyyy HH: mm"),
                ModifiedDateTime = p.ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
                FirstName = ep2.FirstName?.ToResponse(),
                LastName = ep2.LastName?.ToResponse(),
                Blogs = ep2.Blogs?.Select(x => x.Id),
            };
        }
    }

}
