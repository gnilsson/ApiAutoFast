
#nullable enable

using ApiAutoFast;
using ApiAutoFast.EntityFramework;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace ApiAutoFast.Sample.Server;

public abstract class CreateBlogEndpoint : EndpointBase<BlogCreateCommand, BlogResponse, BlogMappingProfile>
{
    private readonly AutoFastSampleDbContext _dbContext;

    public CreateBlogEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        MapRoute("/blogs", HttpVerb.Post);
        AllowAnonymous();
    }

    public override async Task HandleRequestAsync(BlogCreateCommand req, CancellationToken ct)
    {
        var entity = Map.ToDomainEntity(req, AddError);

        if (HasError())
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        await _dbContext.AddAsync(entity, ct);

        if (ShouldSave())
        {
            await _dbContext.SaveChangesAsync(ct);
        }

        var response = Map.FromEntity(entity);

        await SendCreatedAtAsync<GetByIdBlogEndpoint>(new { Id = entity.Id }, response, generateAbsoluteUrl: true, cancellation: ct);
    }
}
