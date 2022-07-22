
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

public partial class UpdatePostEndpoint : EndpointBase<PostModifyCommand, PostResponse, PostMappingProfile>
{
    private readonly AutoFastSampleDbContext _dbContext;

    public UpdatePostEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        MapRoute("/posts/{id}", HttpVerb.Put);
        AllowAnonymous();
    }

    public override Task HandleAsync(PostModifyCommand req, CancellationToken ct)
    {
        return HandleRequestAsync(req, ct);
    }

    public override async Task HandleRequestAsync(PostModifyCommand req, CancellationToken ct)
    {
        var identifier = IdentifierUtility.ConvertFromRequest<Identifier>(req.Id, AddError);

        if (HasError())
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        var result = await _dbContext.Posts.FindAsync(new object?[] { identifier }, cancellationToken: ct);

        if (result is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        result = Map.UpdateDomainEntity(result, req, AddError);

        if (HasError())
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        if (ShouldSave())
        {
            await _dbContext.SaveChangesAsync(ct);
        }

        var response = Map.FromEntity(result);

        await SendOkAsync(response, ct);
    }
}
