//HintName: AuthorMappingProfile.g.cs

using FastEndpoints;

namespace ApiAutoFast.Sample.Server.Database;

public partial class AuthorMappingProfile : Mapper<AuthorCreateCommand, AuthorResponse, Author>
{

    private readonly bool _onOverrideUpdateEntity = false;

    public partial Author OnOverrideUpdateEntity(Author originalEntity, AuthorModifyCommand e);

    public override AuthorResponse FromEntity(Author e)
    {
        return e.AdaptToResponse();
    }

    public partial AuthorUpdateEntity(Author originalEntity, AuthorModifyCommand e)
    {
        if(_onOverrideUpdateEntity)
        {
            return OnOverrideUpdateEntity(originalEntity, e);
        }

        return originalEntity;
    }
}
