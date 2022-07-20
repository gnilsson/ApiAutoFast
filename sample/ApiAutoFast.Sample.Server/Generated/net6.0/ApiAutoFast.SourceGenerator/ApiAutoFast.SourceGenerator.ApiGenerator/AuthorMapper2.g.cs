
#nullable enable

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server;

public static partial class AuthorMapper2
{
    public static AuthorResponse2 MapToResponse(this Author p0)
    {
        if(p0 is null) return null!;

        return new AuthorResponse2
        {
            Id = p0.Id.ToString(),
            CreatedDateTime = ((ITimestamp)p0).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH: mm"),
            ModifiedDateTime = ((ITimestamp)p0).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
            FirstName = p0.FirstName?.ToResponse(),
            LastName = p0.LastName?.ToResponse(),
        };
    }

}
