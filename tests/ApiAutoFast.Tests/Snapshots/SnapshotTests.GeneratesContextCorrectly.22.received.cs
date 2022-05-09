//HintName: GetBlogEndpoint.g.cs

using ApiAutoFast;
using FastEndpoints;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ApiAutoFast.Sample.Server.Database;

public partial class GetBlogEndpoint : Endpoint<BlogQueryRequest, PaginatedResponse<BlogResponse>, BlogMappingProfile>
{
    partial void OnExtendConfigure();
    private bool _extendConfigure = false;
    private readonly AutoFastSampleDbContext _dbContext;

    public GetBlogEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        if (_extendConfigure is false)
        {
            Verbs(Http.GET);
            Routes("/blogs");
            AllowAnonymous();
        }

        OnExtendConfigure();
    }

    public override async Task HandleAsync(BlogQueryRequest req, CancellationToken ct)
    {
        var result = await _dbContext.Blogs.Where(x => true).ToArrayAsync(ct);

        if (result.Length == 0)
        {
            await SendNotFoundAsync(ct);

            return;
        }

        var response = result.Select(x => Map.FromEntity(x));

        var pag = new PaginatedResponse<BlogResponse> { Data = response };

        await SendOkAsync(pag, ct);
    }
}
