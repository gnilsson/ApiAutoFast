using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

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