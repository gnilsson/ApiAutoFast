//HintName: DeleteBlogEndpoint.g.cs

using ApiAutoFast;
using FastEndpoints;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ApiAutoFast.Sample.Server.Database;

public partial class DeleteBlogEndpoint : Endpoint<BlogDeleteCommand, BlogResponse, BlogMappingProfile>
{
    partial void OnExtendConfigure();
    private bool _extendConfigure = false;
    private readonly AutoFastSampleDbContext _dbContext;

    public DeleteBlogEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        if (_extendConfigure is false)
        {
            Verbs(Http.DELETE);
            Routes("/blogs/{id}");
            AllowAnonymous();
        }

        OnExtendConfigure();
    }

    public override async Task HandleAsync(BlogDeleteCommand req, CancellationToken ct)
    {
        var result = await _dbContext.Blogs.FindAsync((Identifier)req.Id, ct);

        if (result is null)
        {
            await SendNotFoundAsync(ct);
        }

        _dbContext.Blogs.Remove(result);

        await _dbContext.SaveChangesAsync(ct);

        await SendOkAsync(ct);
    }
}
