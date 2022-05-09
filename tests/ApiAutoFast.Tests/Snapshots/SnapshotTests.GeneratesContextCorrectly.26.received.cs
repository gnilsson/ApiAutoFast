//HintName: UpdateAuthorEndpoint.g.cs

using ApiAutoFast;
using FastEndpoints;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ApiAutoFast.Sample.Server.Database;

public partial class UpdateAuthorEndpoint : Endpoint<AuthorModifyCommand, AuthorResponse, AuthorMappingProfile>
{
    partial void OnExtendConfigure();
    private bool _extendConfigure = false;
    private readonly AutoFastSampleDbContext _dbContext;

    public UpdateAuthorEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        if (_extendConfigure is false)
        {
            Verbs(Http.PUT);
            Routes("/authors/{id}");
            AllowAnonymous();
        }

        OnExtendConfigure();
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
