
using FastEndpoints;

namespace ApiAutoFast.Sample.Server.Database;

public partial class PostMappingProfile : Mapper<PostCreateCommand, PostResponse, Post>
{
    private readonly bool _onOverrideUpdateEntity = false;

    partial void OnOverrideUpdateEntity(ref Post originalEntity, PostModifyCommand e);

    public override PostResponse FromEntity(Post e)
    {
        return e.AdaptToResponse();
    }

    public Post UpdateEntity(Post originalEntity, PostModifyCommand e)
    {
        if(_onOverrideUpdateEntity)
        {
            OnOverrideUpdateEntity(ref originalEntity, e);
            return originalEntity;
        }

        return originalEntity;
    }

    public Post ToDomainEntity(PostCreateCommand command, Action<string, string> addValidationError)
    {
        return new Post
        {
            Title = Title.ConvertFromRequest(command.Title, addValidationError),
            PublicationDateTime = PublicationDateTime.ConvertFromRequest(command.PublicationDateTime, addValidationError),
            Description = Description.ConvertFromRequest(command.Description, addValidationError),
            PostType = PostType.ConvertFromRequest(command.PostType, addValidationError),
            LikeCount = LikeCount.ConvertFromRequest(command.LikeCount, addValidationError),
            BlogId = Identifier.ConvertFromRequest(command.BlogId, addValidationError),
        };
    }
}
