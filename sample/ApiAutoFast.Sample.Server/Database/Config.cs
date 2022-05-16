using FastEndpoints;
using FluentValidation.Results;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace ApiAutoFast.Sample.Server.Database;


[AutoFastEndpoints]
public class PostConfig
{
    public Title Title { get; set; } = default!;
    public PublicationDateTime PublicationDateTime { get; set; } = default!;
    public Description Description { get; set; } = default!;
    public PostType PostType { get; set; } = default!;
    public LikeCount LikeCount { get; set; } = default!;
}


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
}

//public partial class PostMappingProfile
//{

//    public List<ValidationFailure> Test { get; set; } = new();
//    public PostMappingProfile()
//    {
//        Action<string, string> hmm = (a, b) => Test.Add(new ValidationFailure(a, b));
//        Aha(hmm);
//    }

//    public void Aha(Action<string, string> hej)
//    {
//        return;
//    }

//    public Post ToDomainEntity2(PostCreateCommand e, Action<string, string> addError)
//    {
//        return new Post
//        {
//            Title = Title.ConvertFromRequest(e.Title, addError),
//            PublicationDateTime = PublicationDateTime.ConvertFromRequest(e.PublicationDateTime, addError),
//            Description = Description.ConvertFromRequest(e.Description, addError)
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
