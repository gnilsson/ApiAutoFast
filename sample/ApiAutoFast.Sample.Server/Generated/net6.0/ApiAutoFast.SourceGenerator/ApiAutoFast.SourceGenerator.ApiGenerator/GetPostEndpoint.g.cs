
using ApiAutoFast;
using ApiAutoFast.EntityFramework;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace ApiAutoFast.Sample.Server;

public partial class GetPostEndpoint : EndpointBase<PostQueryRequest, Paginated<PostResponse>, PostMappingProfile>
{
    private readonly AutoFastSampleDbContext _dbContext;

    private static readonly Dictionary<string, Func<string, Expression<Func<Post, bool>>>> _stringMethods = new()
    {
        ["Title"] = static query => entity => ((string)entity.Title).Contains(query),
        ["Description"] = static query => entity => ((string)entity.Description).Contains(query),
    };
    private static readonly string[] _relationalNavigationNames = new string[]
    {
        "Blog",
    };
    private readonly IQueryExecutor<Post, Identifier> _queryExecutor;

    public GetPostEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
        _queryExecutor = new QueryExecutor<Post, Identifier>(_dbContext.Posts, _stringMethods, _relationalNavigationNames);
    }

    public override void Configure()
    {
        MapRoute("/posts", HttpVerb.Get);
        AllowAnonymous();
    }

    public override Task HandleAsync(PostQueryRequest req, CancellationToken ct)
    {
        return HandleRequestAsync(req, ct);
    }

    public override async Task HandleRequestAsync(PostQueryRequest req, CancellationToken ct)
    {
        var response = YieldResponse(_queryExecutor.ExecuteAsync(HttpContext.Request.Query, ct));

        await SendOkAsync(new Paginated<PostResponse> { Data = response }, ct);

        static async IAsyncEnumerable<PostResponse> YieldResponse(IAsyncEnumerable<Post> entities)
        {
            await foreach (var entity in entities)
            {
                yield return Map.FromEntity(entity);
            }
        }
    }
}
