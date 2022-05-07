﻿//HintName: UpdateAuthorEndpoint.g.cs

using ApiAutoFast;
using FastEndpoints;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ApiAutoFast.Sample.Server.Database;


public partial class UpdateAuthorEndpoint : Endpoint<AuthorModifyCommandAuthorResponseAuthorMappingProfile>
{
    partial void ExtendConfigure();
    private bool _overrideConfigure = false;
    private readonly AutoFastSampleDbContext _dbContext;

    public UpdateAuthorEndpointEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        if (_overrideConfigure is false)
        {
            Verbs(Http.PUT);
            Routes("/authors");

            AllowAnonymous();
        }

        ExtendConfigure();
    }

    public override async Task HandleAsync(AuthorModifyCommand req, CancellationToken ct)
    {
        return base.HandleAsync(req, ct);
    }
}
