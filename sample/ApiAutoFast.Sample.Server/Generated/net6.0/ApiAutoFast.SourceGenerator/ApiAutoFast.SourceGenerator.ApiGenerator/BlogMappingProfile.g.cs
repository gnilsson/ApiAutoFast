
using FastEndpoints;

namespace ApiAutoFast.Sample.Server;

public partial class BlogMappingProfile : Mapper<BlogCreateCommand, BlogResponse, Blog>
{
    public override BlogResponse FromEntity(Blog e)
    {
        return e.MapToResponse();
    }

    public Blog UpdateDomainEntity(Blog entity, BlogModifyCommand command, Action<string, string> addValidationError)
    {
        entity.Title = Title.UpdateFromRequest(entity.Title, command.Title, addValidationError);
        return entity;
    }

    public Blog ToDomainEntity(BlogCreateCommand command, Action<string, string> addValidationError)
    {
        return new Blog
        {
            Title = Title.ConvertFromRequest(command.Title, addValidationError),
        };
    }
}
