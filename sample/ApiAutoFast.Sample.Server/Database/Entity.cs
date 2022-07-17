using Microsoft.EntityFrameworkCore;

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
}


public class BlogRelation : DomainValue<string, Blog, BlogRelation>
{

}

public class PostsRelation : DomainValue<ICollection<Post>, PostsRelation>
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

public abstract class AutoFastConfiguration
{

}

public class AutoFastSampleConfiguration : AutoFastConfiguration
{

}