using Microsoft.EntityFrameworkCore;

namespace ApiAutoFast.Sample.Server;


//[AutoFastEndpoints]
//public class PostEntity
//{
//    public Title Title { get; set; } = default!;
//    public PublicationDateTime PublicationDateTime { get; set; } = default!;
//    public Description Description { get; set; } = default!;
//    public PostType PostType { get; set; } = default!;
//    public LikeCount LikeCount { get; set; } = default!;
//    public BlogRelation Blog { get; set; } = default!;
//}
//[AutoFastEndpoints(IdType = IdType.Sequential)]

//[AutoFastEntity(idType: IdType.Identifier)]

[AutoFastEntity(idType: IdType.SequentialIdentifier)]
public class BlogEntity
{
    public Title Title { get; set; } = default!;
    // public PostsRelation Posts { get; set; } = default!;
}

//public class Title2 : StringDomainValue<Title2>
//{
//    public const string RegexPattern = @"^[a-zA-Z0-9 ]*$";

//    public static implicit operator string(Title2 domain) => domain.EntityValue;
//}


[AutoFastEndpoint]
public class CreateBlogEndpointExtended : CreateBlogEndpoint
{
    public CreateBlogEndpointExtended(AutoFastSampleDbContext dbContext) : base(dbContext)
    {
    }

    public override Task HandleAsync(BlogCreateCommand req, CancellationToken ct)
    {
        return _handleRequestAsync(req, ct);
    }
}

// note: if first generic param is specific in foreign relation it will be available for query/command
//       if its a to-many relation ...
//public class BlogRelation : DomainValue<string, Blog, BlogRelation>
//{

//}

//public class PostsRelation : DomainValue<ICollection<Post>, PostsRelation>
//{

//}


[AutoFastContext]
public partial class AutoFastSampleDbContext : DbContext
{
    //partial void ExtendOnModelCreating(ModelBuilder modelBuilder)
    //{

    //}
}

public partial class MappingRegister
{
    //static partial void ExtendRegister(CodeGenerationConfig config)
    //{

    //}
}
