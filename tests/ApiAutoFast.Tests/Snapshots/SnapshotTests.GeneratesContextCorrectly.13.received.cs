//HintName: GetByIdAuthorEndpoint.g.cs

using ApiAutoFast;
using FastEndpoints;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ApiAutoFast.Sample.Server.Database;


public partial class GetByIdAuthorEndpoint : Endpoint<AuthorGetByIdRequestAuthorResponseAuthorMappingProfile>
{
    partial void ExtendConfigure();
    private bool _overrideConfigure = false;
    private readonly AutoFastSampleDbContext _dbContext;

    public GetByIdAuthorEndpointEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        if (_overrideConfigure is false)
        {
            Verbs(Http.GET);
            Routes("/authors");

            AllowAnonymous();
        }

        ExtendConfigure();
    }

    public override async Task HandleAsync(AuthorGetByIdRequest req, CancellationToken ct)
    {
        return base.HandleAsync(req, ct);
    }
}
