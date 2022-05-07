
using ApiAutoFast;
using FastEndpoints;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ApiAutoFast.Sample.Server.Database;
public partial class GetByIdAuthorEndpoint : Endpoint<AuthorGetByIdRequest, AuthorResponse, AuthorMappingProfile>
{
    partial void ExtendConfigure();
    private bool _overrideConfigure = false;
    private readonly AutoFastSampleDbContext _dbContext;

    public GetByIdAuthorEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        if (_overrideConfigure is false)
        {
            Verbs(Http.GET);
            Routes("/authors/{id}");

            AllowAnonymous();
        }

        ExtendConfigure();
    }

    public override async Task HandleAsync(AuthorGetByIdRequest req, CancellationToken ct)
    {
        var result = await _dbContext.Authors.FindAsync(new [] { req.Id }, ct);

        if (result is null)
        {
            await SendNotFoundAsync(ct);
        }

        var response = Map.FromEntity(result);

        await SendOkAsync(response, ct);
    }
}
