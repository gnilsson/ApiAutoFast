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
    // note: will have a seperate implementation model with marker attributes
    //       this is nessescary to find data types in source generator
    //public class Properties
    //{
    //    public class PublicationDateTime : DomainValue<string, DateTime, PublicationDateTime>
    //    {
    //        protected override bool TryValidateRequestConversion(string? requestValue, out DateTime entityValue)
    //        {
    //            entityValue = default!;
    //            return requestValue is not null && DateTime.TryParse(requestValue, out entityValue);
    //        }

    //        // note: figure this one out
    //        public override string ToString() => EntityValue.ToLongDateString();
    //    }


    //    public class Title : DomainValue<string, Title>
    //    {
    //        private const string RegexPattern = "";

    //        protected override bool TryValidateRequestConversion(string? requestValue, out string entityValue)
    //        {
    //            entityValue = requestValue!;
    //            return requestValue is not null && Regex.IsMatch(requestValue, RegexPattern);
    //        }

    //        protected override string? MessageOnFailedValidation => "Incorrect format on Title.";
    //    }

    //    public class Description : DomainValue<string, Description>
    //    { }

    //    public class PostType : DomainValue<EPostType, PostType>
    //    { }
    //}


    public Title Title { get; set; } = default!;
    public PublicationDateTime PublicationDateTime { get; set; } = default!;
    public Description Description { get; set; } = default!;
    public PostType PostType { get; set; } = default!;

    //public class APostType : DomainValue<EPostType, PostType>
    //{

    //}
}

public class TestPost
{
    public object AdaptToResponse()
    {
        return null!;
    }


}


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
//    //public override Post ToEntity(PostCreateCommand postCreateCommand)
//    //{

//    //}
//    //public List<ValidationFailure> Test { get; set; } = new();
//    //public PostMappingProfile()
//    //{
//    //    Action<string, string> hmm = (a, b) => Test.Add(new ValidationFailure(a, b));
//    //    Aha(hmm);
//    //}

//    //public void Aha(Action<string, string> hej)
//    //{
//    //    return;
//    //}

//    //public Post ToDomainEntity(PostCreateCommand e, Action<string, string> addError)
//    //{
//    //    return new Post
//    //    {
//    //        Title = Title.ConvertFromRequest(e.Title, addError),
//    //        PublicationDateTime = PublicationDateTime.ConvertFromRequest(e.PublicationDateTime, addError),
//    //        Description = Description.ConvertFromRequest(e.Description, addError)
//    //    };
//    //}
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
