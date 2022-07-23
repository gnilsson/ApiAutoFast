namespace ApiAutoFast.Sample.Server;


[AutoFastEndpoint]
public class MyCustom : CreateBlogEndpoint
{
    public MyCustom(AutoFastSampleDbContext dbContext) : base(dbContext)
    { }

    public override Task HandleAsync(BlogCreateCommand req, CancellationToken ct)
    {
        return HandleRequestAsync(req, ct);
    }
}