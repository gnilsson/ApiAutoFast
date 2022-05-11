//HintName: CreateAuthorEndpoint.g.cs

using ApiAutoFast;
using FastEndpoints;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ApiAutoFast.Sample.Server.Database;

public partial class CreateAuthorEndpoint : Endpoint<AuthorCreateCommand, AuthorResponse, AuthorMappingProfile>
{
    partial void ExtendConfigure();
    private bool _overrideConfigure = false;
    private readonly AutoFastSampleDbContext _dbContext;

    public CreateAuthorEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        if (_overrideConfigure is false)
        {
            Verbs(Http.POST);
            Routes("/authors");
            // note: temporarily allow anonymous
            AllowAnonymous();
        }

        ExtendConfigure();
    }

    public override async Task HandleAsync(AuthorCreateCommand req, CancellationToken ct)
    {
        var entity = Map.ToEntity(req);

        await _dbContext.AddAsync(entity, ct);

        await _dbContext.SaveChangesAsync(ct);

        var response = Map.FromEntity(entity);

        await SendCreatedAtAsync<GetByIdAuthorEndpoint>(new { Id = response.Id }, response, generateAbsoluteUrl: true, cancellation: ct);
    }
}
