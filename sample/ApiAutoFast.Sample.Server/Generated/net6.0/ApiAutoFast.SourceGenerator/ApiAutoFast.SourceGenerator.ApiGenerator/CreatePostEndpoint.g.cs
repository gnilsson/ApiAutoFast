﻿
using ApiAutoFast;
using FastEndpoints;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ApiAutoFast.Sample.Server.Database;

public partial class CreatePostEndpoint : Endpoint<PostCreateCommand, PostResponse, PostMappingProfile>
{
    partial void ExtendConfigure();
    private bool _overrideConfigure = false;
    private readonly AutoFastSampleDbContext _dbContext;

    public CreatePostEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        if (_overrideConfigure is false)
        {
            Verbs(Http.POST);
            Routes("/posts");
            // note: temporarily allow anonymous
            AllowAnonymous();
        }

        ExtendConfigure();
    }

    public override async Task HandleAsync(PostCreateCommand req, CancellationToken ct)
    {
        var entity = Map.ToDomainEntity(
            req,
            (paramName, message) => ValidationFailures.Add(new FluentValidation.Results.ValidationFailure(paramName, message)));

        await _dbContext.AddAsync(entity, ct);

        await _dbContext.SaveChangesAsync(ct);

        var response = Map.FromEntity(entity);

        await SendCreatedAtAsync<GetByIdPostEndpoint>(new { Id = response.Id }, response, generateAbsoluteUrl: true, cancellation: ct);
    }
}
