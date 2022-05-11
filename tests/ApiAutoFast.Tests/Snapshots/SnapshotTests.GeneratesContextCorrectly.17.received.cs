//HintName: UpdateAuthorEndpoint.g.cs

using ApiAutoFast;
using FastEndpoints;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ApiAutoFast.Sample.Server.Database;

public partial class UpdateAuthorEndpoint : Endpoint<AuthorModifyCommand, AuthorResponse, AuthorMappingProfile>
{
    partial void ExtendConfigure();
    private bool _overrideConfigure = false;
    private readonly AutoFastSampleDbContext _dbContext;

    public UpdateAuthorEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        if (_overrideConfigure is false)
        {
            Verbs(Http.PUT);
            Routes("/authors/{id}");
            // note: temporarily allow anonymous
            AllowAnonymous();
        }

        ExtendConfigure();
    }

    public override async Task HandleAsync(AuthorModifyCommand req, CancellationToken ct)
    {
        var result = await _dbContext.Authors.FindAsync((Identifier)req.Id, ct);

        if (result is null)
        {
            await SendNotFoundAsync(ct);
        }

        var entity = Map.UpdateEntity(result, req);

        var response = Map.FromEntity(entity);

        await SendOkAsync(response, ct);
    }
}
