
using ApiAutoFast;
using ApiAutoFast.EntityFramework;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace ApiAutoFast.Sample.Server;

public partial class DeleteBlogEndpoint : EndpointBase<BlogDeleteCommand, BlogResponse, BlogMappingProfile>
{
    private readonly AutoFastSampleDbContext _dbContext;

    public DeleteBlogEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        MapRoute("/blogs/{id}", HttpVerb.Delete);
        AllowAnonymous();
    }

    public override Task HandleAsync(BlogDeleteCommand req, CancellationToken ct)
    {
        return HandleRequestAsync(req, ct);
    }

    public override async Task HandleRequestAsync(BlogDeleteCommand req, CancellationToken ct)
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

        _dbContext.Blogs.Remove(result);

        if (ShouldSave())
        {
            await _dbContext.SaveChangesAsync(ct);
        }

        await SendOkAsync(ct);
    }
}
