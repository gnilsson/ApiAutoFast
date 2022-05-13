//HintName: GetAbcEndpoint.g.cs

using ApiAutoFast;
using FastEndpoints;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ApiAutoFast.Sample.Server.Database;

public partial class GetAbcEndpoint : Endpoint<AbcQueryRequest, Paginated<AbcResponse>, AbcMappingProfile>
{
    partial void ExtendConfigure();
    private bool _overrideConfigure = false;
    private readonly AutoFastSampleDbContext _dbContext;

    public GetAbcEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        if (_overrideConfigure is false)
        {
            Verbs(Http.GET);
            Routes("/abcs");
            // note: temporarily allow anonymous
            AllowAnonymous();
        }

        ExtendConfigure();
    }

    public override async Task HandleAsync(AbcQueryRequest req, CancellationToken ct)
    {
        var result = await _dbContext.Abcs.Where(x => true).ToArrayAsync(ct);

        if (result.Length == 0)
        {
            await SendNotFoundAsync(ct);

            return;
        }

        var response = result.Select(x => Map.FromEntity(x));

        var pag = new Paginated<AbcResponse> { Data = response };

        await SendOkAsync(pag, ct);
    }
}
