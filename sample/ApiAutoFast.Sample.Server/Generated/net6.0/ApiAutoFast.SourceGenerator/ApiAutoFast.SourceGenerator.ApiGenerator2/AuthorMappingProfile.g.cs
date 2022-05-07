
using FastEndpoints;

namespace ApiAutoFast.Sample.Server.Database;

public partial class AuthorMappingProfile : Mapper<AuthorCreateCommand, AuthorResponse, Author>
{
    public override AuthorResponse FromEntity(Author e)
    {
        return e.AdaptToResponse();
    }
}
