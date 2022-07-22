
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

public partial class UpdateBlogEndpoint : EndpointBase<BlogModifyCommand, BlogResponse, BlogMappingProfile>
{
    private readonly AutoFastSampleDbContext _dbContext;

    public UpdateBlogEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        MapRoute("/blogs/{id}", HttpVerb.Put);
        AllowAnonymous();
    }

    public override Task HandleAsync(BlogModifyCommand req, CancellationToken ct)
    {
        return HandleRequestAsync(req, ct);
    }

    public override async Task HandleRequestAsync(BlogModifyCommand req, CancellationToken ct)
    {
        var identifier = IdentifierUtility.ConvertFromRequest<SequentialIdentifier>(req.Id, AddError);

        if (HasError())
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        var result = await _dbContext.Blogs.FindAsync(new object?[] { identifier }, cancellationToken: ct);

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
