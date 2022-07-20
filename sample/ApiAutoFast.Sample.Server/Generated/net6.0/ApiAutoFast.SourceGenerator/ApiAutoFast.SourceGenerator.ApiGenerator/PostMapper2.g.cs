
#nullable enable

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server;

public static partial class PostMapper2
{
    public static PostResponse2 MapToResponse(this Post p0)
    {
        if(p0 is null) return null!;

        return new PostResponse2
        {
            Id = p0.Id.ToString(),
            CreatedDateTime = ((ITimestamp)p0).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH: mm"),
            ModifiedDateTime = ((ITimestamp)p0).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
            Title = p0.Title?.ToResponse(),
            PublicationDateTime = p0.PublicationDateTime?.ToResponse(),
            Description = p0.Description?.ToResponse(),
            PostType = p0.PostType?.ToResponse(),
            LikeCount = p0.LikeCount?.ToResponse(),
            Blog = MapSimple0(p0.Blog),
        };
    }

    private static BlogResponseSimplified MapSimple0(Blog p1)
    {
        if(p1 is null) return null!;

        return new BlogResponseSimplified
        {
            Id = p1.Id.ToString(),
            CreatedDateTime = ((ITimestamp)p1).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH: mm"),
            ModifiedDateTime = ((ITimestamp)p1).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
            Title = p1.Title?.ToResponse(),
        };
    }

}
