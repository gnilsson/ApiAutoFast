
using ApiAutoFast;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace ApiAutoFast.Sample.Server.Database;

public partial class CreateBlogEndpoint : Endpoint<BlogCreateCommand, BlogResponse, BlogMappingProfile>
{
    partial void ExtendConfigure();
    private bool _overrideConfigure = false;
    private readonly AutoFastSampleDbContext _dbContext;

    public CreateBlogEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        if (_overrideConfigure is false)
        {
            Verbs(Http.POST);
            Routes("/blogs");
            // note: temporarily allow anonymous
            AllowAnonymous();
        }

        ExtendConfigure();
    }

    public override async Task HandleAsync(BlogCreateCommand req, CancellationToken ct)
    {
        var entity = Map.ToDomainEntity(
            req,
            (paramName, message) => ValidationFailures.Add(new ValidationFailure(paramName, message)));

        if (ValidationFailures.Count > 0)
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        await _dbContext.AddAsync(entity, ct);

        await _dbContext.SaveChangesAsync(ct);

        var response = Map.FromEntity(entity);

        await SendCreatedAtAsync<GetByIdBlogEndpoint>(new { Id = entity.Id }, response, generateAbsoluteUrl: true, cancellation: ct);
    }
}
