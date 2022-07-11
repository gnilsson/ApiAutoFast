
using FastEndpoints;

namespace ApiAutoFast.Sample.Server;

public partial class BlogMappingProfile : Mapper<BlogCreateCommand, BlogResponse, Blog>
{
    private readonly bool _onOverrideUpdateEntity = false;

    partial void OnOverrideUpdateEntity(ref Blog originalEntity, BlogModifyCommand e);

    public override BlogResponse FromEntity(Blog e)
    {
        return e.AdaptToResponse();
    }

    public Blog UpdateEntity(Blog originalEntity, BlogModifyCommand e)
    {
        if(_onOverrideUpdateEntity)
        {
            OnOverrideUpdateEntity(ref originalEntity, e);
            return originalEntity;
        }

        return originalEntity;
    }

    public Blog ToDomainEntity(BlogCreateCommand command, Action<string, string> addValidationError)
    {
        return new Blog
        {
            Title = Title.ConvertFromRequest<Title>(command.Title, addValidationError),
        };
    }
}
