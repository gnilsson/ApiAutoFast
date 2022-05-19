
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
    private readonly AutoFastSampleDbContext _dbContext;
    private bool _overrideConfigure = false;
    private bool _saveChanges = true;
    private Blog _entity;

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
        _entity = Map.ToDomainEntity(req, AddError);

        if (HasError())
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        await _dbContext.AddAsync(_entity, ct);

        if (_saveChanges)
        {
            await _dbContext.SaveChangesAsync(ct);
        }

        var response = Map.FromEntity(_entity);

        await SendCreatedAtAsync<GetByIdBlogEndpoint>(new { Id = _entity.Id }, response, generateAbsoluteUrl: true, cancellation: ct);
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
