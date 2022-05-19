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
