﻿
using ApiAutoFast;
using FastEndpoints;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ApiAutoFast.Sample.Server.Database;

public partial class DeleteBlogEndpoint : Endpoint<BlogDeleteCommand, BlogResponse, BlogMappingProfile>
{
    partial void ExtendConfigure();
    private bool _overrideConfigure = false;
    private readonly AutoFastSampleDbContext _dbContext;

    public DeleteBlogEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        if (_overrideConfigure is false)
        {
            Verbs(Http.DELETE);
            Routes("/blogs/{id}");
            // note: temporarily allow anonymous
            AllowAnonymous();
        }

        ExtendConfigure();
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