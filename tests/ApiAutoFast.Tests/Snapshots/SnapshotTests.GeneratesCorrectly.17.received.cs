//HintName: PostMapper2.g.cs

#nullable enable

using ApiAutoFast;
using System.Linq;

namespace ApiAutoFast.Sample.Server;

public static partial class PostMapper2
{
    public static PostResponse MapToResponse(this Post p0)
    {
        if(p0 is null) return null!;

        return new PostResponse
        {
            Id = p0.Id.ToString(),
            CreatedDateTime = ((ITimestamp)p0).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH: mm"),
            ModifiedDateTime = ((ITimestamp)p0).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
            LikeCount = p0.LikeCount?.ToResponse(),
            BlogId = p0.BlogId?.ToResponse(),
            Blog = MapSimple0(p0.Blog),
            Tit = p0.Tit?.ToResponse(),
            PublicationDateTime = p0.PublicationDateTime?.ToResponse(),
            Description = p0.Description?.ToResponse(),
            PostType = p0.PostType?.ToResponse(),
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
