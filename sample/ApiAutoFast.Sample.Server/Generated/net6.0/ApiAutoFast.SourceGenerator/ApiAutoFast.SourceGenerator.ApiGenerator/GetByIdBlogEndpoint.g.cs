﻿
using ApiAutoFast;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace ApiAutoFast.Sample.Server.Database;

public partial class GetByIdBlogEndpoint : Endpoint<BlogGetByIdRequest, BlogResponse, BlogMappingProfile>
{
    partial void ExtendConfigure();
    private bool _overrideConfigure = false;
    private readonly AutoFastSampleDbContext _dbContext;

    public GetByIdBlogEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        if (_overrideConfigure is false)
        {
            Verbs(Http.GET);
            Routes("/blogs/{id}");
            // note: temporarily allow anonymous
            AllowAnonymous();
        }

        ExtendConfigure();
    }

    public override async Task HandleAsync(BlogGetByIdRequest req, CancellationToken ct)
    {
        if (Identifier.TryParse(req.Id, out var identifier) is false)
        {
            ValidationFailures.Add(new ValidationFailure(nameof(req.Id), "Incorrect format on identifier."));
            await SendErrorsAsync(400, ct);
            return;
        }

        var result = await _dbContext.Blogs.FindAsync(identifier);

        if (result is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var response = Map.FromEntity(result);

        await SendOkAsync(response, ct);
    }
}
