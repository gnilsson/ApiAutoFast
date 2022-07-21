
#nullable enable

using ApiAutoFast;
using System.Linq;

namespace ApiAutoFast.Sample.Server;

public static partial class AuthorMapper2
{
    public static AuthorResponse MapToResponse(this Author p)
    {
        if (p is null) return null!;

        return new AuthorResponse
        {
            Id = p.Id.ToString(),
            CreatedDateTime = p.CreatedDateTime.ToString("dddd, dd MMMM yyyy HH: mm"),
            ModifiedDateTime = p.ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
            FirstName = p.FirstName?.ToResponse(),
            LastName = p.LastName?.ToResponse(),
            Blogs = MapEnumerable0(p.Blogs),
        };
    }

    private static System.Collections.Generic.IEnumerable<BlogResponseSimplified> MapEnumerable0(System.Collections.Generic.IEnumerable<Blog> ep)
    {
        if (ep is null) yield break;

        foreach (var p in ep)
        {
            yield return new BlogResponseSimplified
            {
                Id = p.Id.ToString(),
                CreatedDateTime = p.CreatedDateTime.ToString("dddd, dd MMMM yyyy HH: mm"),
                ModifiedDateTime = p.ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
                Title = p.Title?.ToResponse(),
                Posts = p.Posts?.Select(x => x.Id.ToString()),
                Authors = p.Authors?.Select(x => x.Id.ToString()),
            };
        }
    }

}
