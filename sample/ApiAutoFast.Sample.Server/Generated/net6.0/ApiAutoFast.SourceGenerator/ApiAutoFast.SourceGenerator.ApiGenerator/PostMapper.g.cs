
#nullable enable

using ApiAutoFast;
using System.Linq;

namespace ApiAutoFast.Sample.Server;

public static partial class PostMapper
{
    public static PostResponse MapToResponse(this Post p)
    {
        if (p is null) return null!;

        return new PostResponse
        {
            Id = p.Id.ToString(),
            CreatedDateTime = p.CreatedDateTime.ToString("dddd, dd MMMM yyyy HH: mm"),
            ModifiedDateTime = p.ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
            Title = p.Title?.ToResponse(),
            PublicationDateTime = p.PublicationDateTime?.ToResponse(),
            Description = p.Description?.ToResponse(),
            PostType = p.PostType?.ToResponse(),
            LikeCount = p.LikeCount?.ToResponse(),
            Blog = MapSimple0(p.Blog),
        };
    }

    private static BlogResponseSimplified MapSimple0(Blog p)
    {
        if (p is null) return null!;

        return new BlogResponseSimplified
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
