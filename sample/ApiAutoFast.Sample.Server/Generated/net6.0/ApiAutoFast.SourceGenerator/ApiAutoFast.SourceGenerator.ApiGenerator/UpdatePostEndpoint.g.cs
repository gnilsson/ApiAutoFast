
using ApiAutoFast;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace ApiAutoFast.Sample.Server.Database;

public partial class UpdatePostEndpoint : Endpoint<PostModifyCommand, PostResponse, PostMappingProfile>
{
    partial void ExtendConfigure();
    private bool _overrideConfigure = false;
    private readonly AutoFastSampleDbContext _dbContext;

    public UpdatePostEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        if (_overrideConfigure is false)
        {
            Verbs(Http.PUT);
            Routes("/posts/{id}");
            // note: temporarily allow anonymous
            AllowAnonymous();
        }

        ExtendConfigure();
    }

    public override async Task HandleAsync(PostModifyCommand req, CancellationToken ct)
    {
        if (Identifier.TryParse(req.Id, out var identifier) is false)
        {
            ValidationFailures.Add(new ValidationFailure(nameof(req.Id), "Incorrect format on identifier."));
            await SendErrorsAsync(400, ct);
            return;
        }

        var result = await _dbContext.Posts.FindAsync(identifier);

        if (result is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var entity = Map.UpdateEntity(result, req);

        var response = Map.FromEntity(entity);

        await SendOkAsync(response, ct);
    }
}
