//HintName: PostMappingProfile.g.cs

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

    public Post ToDomainEntity(PostCreateCommand e, Action<string, string> addError)
    {
        return new Post
        {
            Title = Title.ConvertFromRequest(e.Title, addError),
            PublicationDateTime = PublicationDateTime.ConvertFromRequest(e.PublicationDateTime, addError),
            Description = Description.ConvertFromRequest(e.Description, addError)
        };
    }
}
