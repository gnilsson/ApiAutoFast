//HintName: AuthorMappingProfile.g.cs

using FastEndpoints;

namespace ApiAutoFast.Sample.Server.Database;

public partial class AuthorMappingProfile : Mapper<AuthorCreateCommand, AuthorResponse, Author>
{
    private readonly bool _onOverrideUpdateEntity = false;

    partial void OnOverrideUpdateEntity(ref Author originalEntity, AuthorModifyCommand e);

    public override AuthorResponse FromEntity(Author e)
    {
        return e.AdaptToResponse();
    }

    public Author UpdateEntity(Author originalEntity, AuthorModifyCommand e)
    {
        if(_onOverrideUpdateEntity)
        {
            OnOverrideUpdateEntity(originalEntity, e);
            return originalEntity;
        }

        return originalEntity;
    }
}
