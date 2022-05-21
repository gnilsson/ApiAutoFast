
using ApiAutoFast;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace ApiAutoFast.Sample.Server;

public partial class GetPostEndpoint : Endpoint<PostQueryRequest, Paginated<PostResponse>, PostMappingProfile>
{
    partial void ExtendConfigure();
    private readonly AutoFastSampleDbContext _dbContext;
    private bool _overrideConfigure = false;

    public GetPostEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        if (_overrideConfigure is false)
        {
            Verbs(Http.GET);
            Routes("/posts");
            // note: temporarily allow anonymous
            AllowAnonymous();
        }

        ExtendConfigure();
    }

    public override async Task HandleAsync(PostQueryRequest req, CancellationToken ct)
    {
        var result = await _dbContext.Posts.Where(x => true).ToArrayAsync(ct);

        if (result.Length == 0)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var response = result.Select(x => Map.FromEntity(x));

        var pag = new Paginated<PostResponse> { Data = response };

        await SendOkAsync(pag, ct);
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
