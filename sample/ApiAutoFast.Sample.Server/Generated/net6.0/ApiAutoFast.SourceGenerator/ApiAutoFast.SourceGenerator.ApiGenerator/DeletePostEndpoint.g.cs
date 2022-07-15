
using ApiAutoFast;
using ApiAutoFast.EntityFramework;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace ApiAutoFast.Sample.Server;

public partial class DeletePostEndpoint : EndpointBase<PostDeleteCommand, PostResponse, PostMappingProfile>
{
    private readonly AutoFastSampleDbContext _dbContext;

    public DeletePostEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        MapRoute("/posts/{id}", HttpVerb.Delete);
        AllowAnonymous();
    }

    public override Task HandleAsync(PostDeleteCommand req, CancellationToken ct)
    {
        return HandleRequestAsync(req, ct);
    }

    public override async Task HandleRequestAsync(PostDeleteCommand req, CancellationToken ct)
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

        _dbContext.Posts.Remove(result);

        if (ShouldSave())
        {
            await _dbContext.SaveChangesAsync(ct);
        }

        await SendOkAsync(ct);
    }
}
