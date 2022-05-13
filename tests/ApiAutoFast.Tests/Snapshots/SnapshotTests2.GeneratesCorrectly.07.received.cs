﻿//HintName: GetByIdPostEndpoint.g.cs

using ApiAutoFast;
using FastEndpoints;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ApiAutoFast.Sample.Server.Database;

public partial class GetByIdPostEndpoint : Endpoint<PostGetByIdRequest, PostResponse, PostMappingProfile>
{
    partial void ExtendConfigure();
    private bool _overrideConfigure = false;
    private readonly AutoFastSampleDbContext _dbContext;

    public GetByIdPostEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        if (_overrideConfigure is false)
        {
            Verbs(Http.GET);
            Routes("/posts/{id}");
            // note: temporarily allow anonymous
            AllowAnonymous();
        }

        ExtendConfigure();
    }

    public override async Task HandleAsync(PostGetByIdRequest req, CancellationToken ct)
    {
        if (Identifier.TryParse(req.Id, out var identifier) is false)
        {
            // todo: think out a good way to do validation. this does not include foreign ids.
            ValidationFailures.Add(new FluentValidation.Results.ValidationFailure(nameof(req.Id), "Incorrect format."));
            await SendErrorsAsync(400, ct);
            return;
        }

        var result = await _dbContext.Posts.FindAsync(identifier);

        if (result is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var response = Map.FromEntity(result);

        await SendOkAsync(response, ct);
    }
}