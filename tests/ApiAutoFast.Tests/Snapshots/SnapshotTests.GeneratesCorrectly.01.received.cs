//HintName: AuthorMapper2.g.cs

#nullable enable

using ApiAutoFast;
using System.Linq;

namespace ApiAutoFast.Sample.Server;

public static partial class AuthorMapper2
{
    public static AuthorResponse MapToResponse(this Author p0)
    {
        if(p0 is null) return null!;

        return new AuthorResponse
        {
            Id = p0.Id.ToString(),
            CreatedDateTime = ((ITimestamp)p0).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH: mm"),
            ModifiedDateTime = ((ITimestamp)p0).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
            FirstName = p0.FirstName?.ToResponse(),
            LastName = p0.LastName?.ToResponse(),
            Blogs = MapSimple0(p0.Blogs),
        };
    }

    private static BlogResponseSimplified MapSimple0(Blog p1)
    {
        if(p1 is null) return null!;

        return new BlogResponseSimplified
        {
            Id = p1.Id.ToString(),
            CreatedDateTime = p1.CreatedDateTime.ToString("dddd, dd MMMM yyyy HH: mm"),
            ModifiedDateTime = p1.ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
            Title = p1.Title?.ToResponse(),
            Posts = p1.PostsRelation?.Select(x => x.Id),
            Authors = p1.AuthorsRelation?.Select(x => x.Id),
        };
    }
}
