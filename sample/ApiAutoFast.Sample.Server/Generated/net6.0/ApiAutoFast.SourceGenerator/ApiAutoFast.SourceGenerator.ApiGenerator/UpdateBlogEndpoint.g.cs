
using ApiAutoFast;
using ApiAutoFast.EntityFramework;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace ApiAutoFast.Sample.Server;

public partial class UpdateBlogEndpoint : Endpoint<BlogModifyCommand, BlogResponse, BlogMappingProfile>
{
    partial void ExtendConfigure();
    private readonly AutoFastSampleDbContext _dbContext;
    private bool _overrideConfigure = false;
    private readonly QueryExecutor<Blog> _queryExecutor;


    public UpdateBlogEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        if (_overrideConfigure is false)
        {
            Verbs(Http.PUT);
            Routes("/blogs/{id}");
            // note: temporarily allow anonymous
            AllowAnonymous();
        }

        ExtendConfigure();
    }

    public override async Task HandleAsync(BlogModifyCommand req, CancellationToken ct)
    {
        var identifier = Identifier.ConvertFromRequest(req.Id, AddError);

        if (HasError())
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        var result = await _dbContext.Blogs.FindAsync(identifier);

        if (result is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var entity = Map.UpdateEntity(result, req);

        var response = Map.FromEntity(entity);

        await SendOkAsync(response, ct);
    }

    private void AddError(string property, string message)
    {
        ValidationFailures.Add(new ValidationFailure(property, message));
    }

    private bool HasError()
    {
        return ValidationFailures.Count > 0;
    }
}
