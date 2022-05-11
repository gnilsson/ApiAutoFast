//HintName: GetAuthorEndpoint.g.cs

using ApiAutoFast;
using FastEndpoints;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ApiAutoFast.Sample.Server.Database;

public partial class GetAuthorEndpoint : Endpoint<AuthorQueryRequest, Paginated<AuthorResponse>, AuthorMappingProfile>
{
    partial void ExtendConfigure();
    private bool _overrideConfigure = false;
    private readonly AutoFastSampleDbContext _dbContext;

    public GetAuthorEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        if (_overrideConfigure is false)
        {
            Verbs(Http.GET);
            Routes("/authors");
            // note: temporarily allow anonymous
            AllowAnonymous();
        }

        ExtendConfigure();
    }

    public override async Task HandleAsync(AuthorQueryRequest req, CancellationToken ct)
    {
        var result = await _dbContext.Authors.Where(x => true).ToArrayAsync(ct);

        if (result.Length == 0)
        {
            await SendNotFoundAsync(ct);

            return;
        }

        var response = result.Select(x => Map.FromEntity(x));

        var pag = new Paginated<AuthorResponse> { Data = response };

        await SendOkAsync(pag, ct);
    }
}
