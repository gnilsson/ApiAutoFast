
using ApiAutoFast;
using ApiAutoFast.EntityFramework;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace ApiAutoFast.Sample.Server;

public partial class GetPostEndpoint : Endpoint<PostQueryRequest, Paginated<PostResponse>, PostMappingProfile>
{
    partial void ExtendConfigure();
    private readonly AutoFastSampleDbContext _dbContext;
    private bool _overrideConfigure = false;
    private readonly QueryExecutor<Post> _queryExecutor;
    private static readonly Dictionary<string, Func<string, Expression<Func<Post, bool>>>> _stringMethods = new()
    {
        ["Title"] = query => entity => ((string)entity.Title).Contains(query),
        ["Description"] = query => entity => ((string)entity.Description).Contains(query),
    };
    private static readonly string[] _relationalNavigationNames = new[]
    {
        "Blog",
    };


    public GetPostEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
        _queryExecutor = new QueryExecutor<Post>(_dbContext.Posts, _stringMethods, _relationalNavigationNames);
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
        var response = YieldResponse(_queryExecutor.ExecuteAsync(HttpContext.Request.Query, ct));

        // return no content

        await SendOkAsync(new Paginated<PostResponse> { Data = response }, ct);

        async IAsyncEnumerable<PostResponse> YieldResponse(IAsyncEnumerable<Post> entities)
        {
            await foreach (var entity in entities)
            {
                yield return Map.FromEntity(entity);
            }
        }
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
