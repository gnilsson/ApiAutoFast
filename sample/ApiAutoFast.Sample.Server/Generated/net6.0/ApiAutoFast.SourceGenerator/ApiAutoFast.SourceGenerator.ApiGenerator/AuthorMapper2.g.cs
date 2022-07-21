
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

    private static BlogResponseSimplified MapEnumerable0(Blog ep1)
    {
        if (ep1 is null) yield break;

        foreach (var p in ep1)
        {
            yield return new BlogResponseSimplified
            {
                Id = p.Id.ToString(),
                CreatedDateTime = p.CreatedDateTime.ToString("dddd, dd MMMM yyyy HH: mm"),
                ModifiedDateTime = p.ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
                Title = ep1.Title?.ToResponse(),
                Posts = ep1.Posts?.Select(x => x.Id),
                Authors = ep1.Authors?.Select(x => x.Id),
            };
        }
    }

}
