
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

public partial class CreatePostEndpoint : EndpointBase<PostCreateCommand, PostResponse, PostMappingProfile>
{
    private readonly AutoFastSampleDbContext _dbContext;

    public CreatePostEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        MapRoute("/posts", HttpVerb.Post);
        AllowAnonymous();
    }

    public override Task HandleAsync(PostCreateCommand req, CancellationToken ct)
    {
        return HandleRequestAsync(req, ct);
    }

    public override async Task HandleRequestAsync(PostCreateCommand req, CancellationToken ct)
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

        await SendCreatedAtAsync<GetByIdPostEndpoint>(new { Id = entity.Id }, response, generateAbsoluteUrl: true, cancellation: ct);
    }
}
