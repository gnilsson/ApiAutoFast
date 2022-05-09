//HintName: GetByIdBlogEndpoint.g.cs

using ApiAutoFast;
using FastEndpoints;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ApiAutoFast.Sample.Server.Database;

public partial class GetByIdBlogEndpoint : Endpoint<BlogGetByIdRequest, BlogResponse, BlogMappingProfile>
{
    partial void OnExtendConfigure();
    private bool _extendConfigure = false;
    private readonly AutoFastSampleDbContext _dbContext;

    public GetByIdBlogEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        if (_extendConfigure is false)
        {
            Verbs(Http.GET);
            Routes("/blogs/{id}");
            AllowAnonymous();
        }

        OnExtendConfigure();
    }

    public override async Task HandleAsync(BlogGetByIdRequest req, CancellationToken ct)
    {
        var result = await _dbContext.Blogs.FindAsync((Identifier)req.Id, ct);

        if (result is null)
        {
            await SendNotFoundAsync(ct);
        }

        var response = Map.FromEntity(result);

        await SendOkAsync(response, ct);
    }
}
