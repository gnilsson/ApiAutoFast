﻿//HintName: DeleteAuthorEndpoint.g.cs

using ApiAutoFast;
using FastEndpoints;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ApiAutoFast.Sample.Server.Database;

public partial class DeleteAuthorEndpoint : Endpoint<AuthorDeleteRequest, AuthorResponse, AuthorMappingProfile>
{
    partial void ExtendConfigure();
    private bool _overrideConfigure = false;
    private readonly AutoFastSampleDbContext _dbContext;

    public DeleteAuthorEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        if (_overrideConfigure is false)
        {
            Verbs(Http.DELETE);
            Routes("/authors/{id}");

            AllowAnonymous();
        }

        ExtendConfigure();
    }

    public override async Task HandleAsync(AuthorDeleteRequest req, CancellationToken ct)
    {
        var result = await _dbContext.Authors.FindAsync(new [] { req.Id }, ct);

        if (result is null)
        {
            await SendNotFoundAsync(ct);
        }

        _dbContext.Authors.Remove(result);

        await _dbContext.SaveChangesAsync(ct);

        await SendOkAsync(ct);
    }
}
