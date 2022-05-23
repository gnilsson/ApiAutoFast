using Mapster;
using Microsoft.EntityFrameworkCore;

namespace ApiAutoFast.Sample.Server;


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


[AutoFastContext]
public partial class AutoFastSampleDbContext : DbContext
{
    partial void ExtendOnModelCreating(ModelBuilder modelBuilder)
    {

    }
}

public partial class MappingRegister
{
    static partial void ExtendRegister(CodeGenerationConfig config)
    {

    }
}

//public static class AdaptAttributeBuilderExtensions
//{
//    public static AdaptAttributeBuilder ForTypeDefaultValues(this AdaptAttributeBuilder aab)
//    {
//        return aab
//            .ForType<Post>(cfg =>
//            {
//                cfg.Map(poco => poco.Id, typeof(string));
//                cfg.Map(poco => poco.CreatedDateTime, typeof(string));
//                cfg.Map(poco => poco.ModifiedDateTime, typeof(string));
//                cfg.Map(poco => poco.Title, typeof(string));
//                cfg.Map(poco => poco.PublicationDateTime, typeof(string));
//                cfg.Map(poco => poco.Description, typeof(string));
//                cfg.Map(poco => poco.PostType, typeof(string));
//                cfg.Map(poco => poco.LikeCount, typeof(int));
//                cfg.Ignore(x => x.BlogId);
//            })
//            .ForType<Blog>(cfg =>
//            {
//                cfg.Map(poco => poco.Id, typeof(string));
//                cfg.Map(poco => poco.CreatedDateTime, typeof(string));
//                cfg.Map(poco => poco.ModifiedDateTime, typeof(string));
//                cfg.Map(poco => poco.Title, typeof(string));
//            });
//    }
//}