
#nullable enable

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server;

public static partial class BlogMapper2
{
    public static BlogResponse2 MapToResponse(this Blog p0)
    {
        if(p0 is null) return null!;

        return new BlogResponse2
        {
            Id = p0.Id.ToString(),
            CreatedDateTime = ((ITimestamp)p0).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH: mm"),
            ModifiedDateTime = ((ITimestamp)p0).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
            Title = p0.Title?.ToResponse(),
        };
    }

}
