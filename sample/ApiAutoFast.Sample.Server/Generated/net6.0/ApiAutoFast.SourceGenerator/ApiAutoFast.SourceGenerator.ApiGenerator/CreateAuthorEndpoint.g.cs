
using ApiAutoFast;
using ApiAutoFast.EntityFramework;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace ApiAutoFast.Sample.Server;

public partial class CreateAuthorEndpoint : EndpointBase<AuthorCreateCommand, AuthorResponse, AuthorMappingProfile>
{
    private readonly AutoFastSampleDbContext _dbContext;

    public CreateAuthorEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        MapRoute("/authors", HttpVerb.Post);
        AllowAnonymous();
    }

    public override Task HandleAsync(AuthorCreateCommand req, CancellationToken ct)
    {
        return HandleRequestAsync(req, ct);
    }

    public override async Task HandleRequestAsync(AuthorCreateCommand req, CancellationToken ct)
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

        await SendCreatedAtAsync<GetByIdAuthorEndpoint>(new { Id = entity.Id }, response, generateAbsoluteUrl: true, cancellation: ct);
    }
}
