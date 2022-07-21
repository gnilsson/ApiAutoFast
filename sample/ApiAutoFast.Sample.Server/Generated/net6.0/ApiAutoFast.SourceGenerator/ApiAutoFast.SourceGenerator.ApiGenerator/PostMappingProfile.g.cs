
using FastEndpoints;

namespace ApiAutoFast.Sample.Server;

public partial class PostMappingProfile : Mapper<PostCreateCommand, PostResponse, Post>
{
    public override PostResponse FromEntity(Post e)
    {
        return e.MapToResponse();
    }

    public Post UpdateDomainEntity(Post entity, PostModifyCommand command, Action<string, string> addValidationError)
    {
        entity.Title = Title.UpdateFromRequest(entity.Title, command.Title, addValidationError);
        entity.PublicationDateTime = PublicationDateTime.UpdateFromRequest(entity.PublicationDateTime, command.PublicationDateTime, addValidationError);
        entity.Description = Description.UpdateFromRequest(entity.Description, command.Description, addValidationError);
        entity.PostType = PostType.UpdateFromRequest(entity.PostType, command.PostType, addValidationError);
        entity.LikeCount = LikeCount.UpdateFromRequest(entity.LikeCount, command.LikeCount, addValidationError);
        entity.BlogId = IdentifierUtility.ConvertFromRequest<SequentialIdentifier>(command.BlogId, addValidationError);
        return entity;
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
            BlogId = IdentifierUtility.ConvertFromRequest<SequentialIdentifier>(command.BlogId, addValidationError),
        };
    }
}
