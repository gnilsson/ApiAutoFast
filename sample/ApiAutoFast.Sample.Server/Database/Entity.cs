using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace ApiAutoFast.Sample.Server.Database;


[AutoFastEndpoints]
public class PostEntity
{
    public Title Title { get; set; } = default!;
    public PublicationDateTime PublicationDateTime { get; set; } = default!;
    public Description Description { get; set; } = default!;
    public PostType PostType { get; set; } = default!;
    public LikeCount LikeCount { get; set; } = default!;
    public BlogRelation Blog { get; set; } = default!;
}

[AutoFastEndpoints]
public class BlogEntity
{
    public Title Title { get; set; } = default!;
    public PostsRelation Posts { get; set; } = default!;
}


// note: if first generic param is specific in foreign relation it will be available for query/command
//       if its a to-many relation ...
public class BlogRelation : DomainValue<string, Blog, BlogRelation>
{

}

public class PostsRelation : DomainValue<ICollection<Post>, PostsRelation>
{

}

//public class Blo2g : IEntity
//{
//    public Blo2g()
//    {
//        Posts = PostsRelation.From(new HashSet<Post>());
//    }

//    public Identifier Id { get; set; }
//    public DateTime CreatedDateTime { get; set; }
//    public DateTime ModifiedDateTime { get; set; }
//    public Title Title { get; set; }
//    public PostsRelation Posts { get; set; }
//}


//public class ForeignTest : DomainValue<string, Test, ForeignTest>
//{

//}


//[AutoFastEndpoints]
//public class TestConfig
//{
//    public Title Title { get; set; } = default!;
//}

[AutoFastContext]
public partial class AutoFastSampleDbContext : DbContext
{
    //public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    //{
    //    return Task.FromResult(0);
    //}
    //partial void ExtendOnModelCreating(ModelBuilder modelBuilder)
    //{
    //    modelBuilder.Entity<Blog>().HasMany<Post>("Posts").WithOne(x => (Blog)x.Blog);
    //}
}

//public partial class PostMappingProfile
//{
//    public Post ToDomainEntity2(PostCreateCommand command, Action<string, string> addValidationError)
//    {
//        return new Post
//        {
//            Title = Title.ConvertFromRequest(command.Title, addValidationError),
//            PublicationDateTime = PublicationDateTime.ConvertFromRequest(command.PublicationDateTime, addValidationError),
//            Description = Description.ConvertFromRequest(command.Description, addValidationError),
//            PostType = PostType.ConvertFromRequest(command.PostType, addValidationError),
//            LikeCount = LikeCount.ConvertFromRequest(command.LikeCount, addValidationError),
//        };
//    }
//}

//public partial class CreatePostEndpoint
//{

//    public override async Task HandleAsync(PostCreateCommand req, CancellationToken ct)
//    {
//        var entity = Map.ToDomainEntity2(
//            req,
//            (paramName, message) => ValidationFailures.Add(new ValidationFailure(paramName, message)));

//        if (ValidationFailures.Count > 0)
//        {
//            await SendErrorsAsync(400, ct);
//            return;
//        }

//        await _dbContext.AddAsync(entity, ct);

//        await _dbContext.SaveChangesAsync(ct);

//        var response = Map.FromEntity(entity);

//        await SendCreatedAtAsync<GetByIdPostEndpoint>(new { Id = entity.Id }, response, generateAbsoluteUrl: true, cancellation: ct);
//    }
//}



//[AutoFastEndpoints(includeEndpointTarget: EndpointTargetType.Get)]
//public class AuthorConfig
//{
//    internal class Properties
//    {
//        public string? FirstName { get; set; }
//        public string? LastName { get; set; }
//        public ProfessionCategory? Profession { get; set; }
//        [ExcludeRequestModel]
//        public ICollection<BlogConfig>? Blogs { get; set; }
//    }
//}

//[AutoFastEndpoints]
//public class BlogConfig
//{
//    internal class Properties
//    {
//        public string? Title { get; set; }
//        [ExcludeRequestModel]
//        public AuthorConfig? Author { get; set; }
//        public Identifier AuthorId { get; set; }
//    }
//}
