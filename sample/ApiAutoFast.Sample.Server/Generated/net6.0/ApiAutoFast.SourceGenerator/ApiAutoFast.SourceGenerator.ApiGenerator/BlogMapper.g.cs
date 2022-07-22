
#nullable enable

using ApiAutoFast;
using System.Linq;

namespace ApiAutoFast.Sample.Server;

public static partial class BlogMapper
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

    private static System.Collections.Generic.IEnumerable<PostResponseSimplified> MapEnumerable0(System.Collections.Generic.IEnumerable<Post> ep)
    {
        if (ep is null) yield break;

        foreach (var p in ep)
        {
            yield return new PostResponseSimplified
            {
                Id = p.Id.ToString(),
                CreatedDateTime = p.CreatedDateTime.ToString("dddd, dd MMMM yyyy HH: mm"),
                ModifiedDateTime = p.ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
                Title = p.Title?.ToResponse(),
                PublicationDateTime = p.PublicationDateTime?.ToResponse(),
                Description = p.Description?.ToResponse(),
                PostType = p.PostType?.ToResponse(),
                LikeCount = p.LikeCount?.ToResponse(),
                Blog = p.BlogId.ToString(),
            };
        }
    }

    private static System.Collections.Generic.IEnumerable<AuthorResponseSimplified> MapEnumerable1(System.Collections.Generic.IEnumerable<Author> ep)
    {
        if (ep is null) yield break;

        foreach (var p in ep)
        {
            yield return new AuthorResponseSimplified
            {
                Id = p.Id.ToString(),
                CreatedDateTime = p.CreatedDateTime.ToString("dddd, dd MMMM yyyy HH: mm"),
                ModifiedDateTime = p.ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
                FirstName = p.FirstName?.ToResponse(),
                LastName = p.LastName?.ToResponse(),
                Blogs = p.Blogs?.Select(x => x.Id.ToString()),
            };
        }
    }

}
