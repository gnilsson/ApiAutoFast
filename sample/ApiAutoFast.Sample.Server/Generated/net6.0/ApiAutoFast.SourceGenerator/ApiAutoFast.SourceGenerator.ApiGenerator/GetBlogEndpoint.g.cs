
using ApiAutoFast;
using ApiAutoFast.EntityFramework;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace ApiAutoFast.Sample.Server;

public partial class GetBlogEndpoint : EndpointBase<BlogQueryRequest, Paginated<BlogResponse>, BlogMappingProfile>
{
    private readonly AutoFastSampleDbContext _dbContext;

    private static readonly Dictionary<string, Func<string, Expression<Func<Blog, bool>>>> _stringMethods = new()
    {
        ["Title"] = static query => entity => ((string)entity.Title).Contains(query),
    };
    private static readonly string[] _relationalNavigationNames = new string[]
    {
        "Posts",
    };
    private readonly IQueryExecutor<Blog, SequentialIdentifier> _queryExecutor;

    public GetBlogEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
        _queryExecutor = new QueryExecutor<Blog, SequentialIdentifier>(_dbContext.Blogs, _stringMethods, _relationalNavigationNames);
    }

    public override void Configure()
    {
        MapRoute("/blogs", HttpVerb.Get);
        AllowAnonymous();
    }

    public override Task HandleAsync(BlogQueryRequest req, CancellationToken ct)
    {
        return HandleRequestAsync(req, ct);
    }

    public override async Task HandleRequestAsync(BlogQueryRequest req, CancellationToken ct)
    {
        var response = YieldResponse(_queryExecutor.ExecuteAsync(HttpContext.Request.Query, ct));

        await SendOkAsync(new Paginated<BlogResponse> { Data = response }, ct);

        static async IAsyncEnumerable<BlogResponse> YieldResponse(IAsyncEnumerable<Blog> entities)
        {
            await foreach (var entity in entities)
            {
                yield return Map.FromEntity(entity);
            }
        }
    }
}
