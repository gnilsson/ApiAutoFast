namespace ApiAutoFast.Sample.Server.Endpoints;


[AutoFastEndpoint]
public class CreateBlogEndpointExtended : CreateBlogEndpoint
{
    public CreateBlogEndpointExtended(AutoFastSampleDbContext dbContext) : base(dbContext)
    {
        var f = dbContext.Set<Blog>();
        var b = dbContext.Blogs;
    }

    public override Task HandleAsync(BlogCreateCommand req, CancellationToken ct)
    {
        return _handleRequestAsync(req, ct);
    }
}
