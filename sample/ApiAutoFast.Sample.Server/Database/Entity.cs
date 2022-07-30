using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ApiAutoFast.Sample.Server;


[AutoFastEntity]
public class PostEntity
{
    public Title Title { get; set; } = default!;
    public PublicationDateTime PublicationDateTime { get; set; } = default!;
    public Description Description { get; set; } = default!;
    public PostType PostType { get; set; } = default!;
    public LikeCount LikeCount { get; set; } = default!;
    public BlogRelation Blog { get; set; } = default!;
}

[AutoFastEntity(idType: IdType.SequentialIdentifier)]
public class BlogEntity
{
    public Title Title { get; set; } = default!;
    public PostsRelation Posts { get; set; } = default!;
    public AuthorsRelation Authors { get; set; } = default!;
}

[AutoFastEntity]
public class AuthorEntity
{
    public FirstName FirstName { get; set; } = default!;
    public LastName LastName { get; set; } = default!;
    public BlogsRelation Blogs { get; set; } = default!;
}



public class Entity
{
    //[AutoFastEntity]
    public class Author : IEntitySetup
    {
        public void Setup()
        {
            this
                .EntitySetup()
                .WithProperty<DomainValue.FirstName>(o => o
                    .WithAttribute<RequiredAttribute>()
                    .WithAttribute<MaxLengthAttribute>(10))
                .WithProperty<LastName>()
                .WithRelationToMany<Blog>();
        }
    }

    public class Blog : IEntitySetup
    {
        public void Setup()
        {
        }
    }
}


public class DomainValue
{
    public class FirstName : IPropertySetup
    {
        public void Setup()
        {
            this
                .PropertySetup()
                .WithDomainValueTypes<string>(o => o.TryValidateRequestConversion = (string? requestValue, out string entityValue) =>
                {
                    entityValue = requestValue!;
                    return requestValue is not null && Regex.IsMatch(requestValue, RegexPattern);
                })
                .WithValidationErrorMessage("Invalid format.");
        }

        private const string RegexPattern = "";
    }

    public class LastName : IPropertySetup
    {
        public void Setup()
        {
            this
                .PropertySetup()
                .WithDomainValueTypes<string>();
        }
    }


}

public class BlogRelation : DomainValue<string, Blog, BlogRelation>
{

}

public class PostsRelation : DomainValue<ICollection<Post>, PostsRelation>
{
}


public class AuthorsRelation : DomainValue<ICollection<Author>, AuthorsRelation>
{

}


public class BlogsRelation : DomainValue<ICollection<Blog>, BlogsRelation>
{

}


[AutoFastContext]
public partial class AutoFastSampleDbContext : DbContext
{
    //partial void ExtendOnModelCreating(ModelBuilder modelBuilder)
    //{

    //}
}

//public class Title2 : StringDomainValue<Title2>
//{
//    public const string RegexPattern = @"^[a-zA-Z0-9 ]*$";
//}


public class AutoFastSampleConfiguration : AutoFastConfiguration
{
    public override void Configure()
    {
        AddValidationErrorMessage<Title>("Incorrect format on Title.");
    }
}