
using ApiAutoFast;
using ApiAutoFast.EntityFramework;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace ApiAutoFast.Sample.Server;

public partial class GetBlogEndpoint : Endpoint<BlogQueryRequest, Paginated<BlogResponse>, BlogMappingProfile>
{
    partial void ExtendConfigure();
    private readonly AutoFastSampleDbContext _dbContext;
    private bool _overrideConfigure = false;
    private readonly QueryExecutor<Blog> _queryExecutor;
    private static readonly Dictionary<string, Func<string, Expression<Func<Blog, bool>>>> _stringMethods = new()
    {
        ["Title"] = query => entity => ((string)entity.Title).Contains(query),
    };
    private static readonly string[] _relationalNavigationNames = new[]
    {
        "Posts",
    };


    public GetBlogEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
        _queryExecutor = new QueryExecutor<Blog>(_dbContext.Blogs, _stringMethods, _relationalNavigationNames);
    }

    public override void Configure()
    {
        if (_overrideConfigure is false)
        {
            Verbs(Http.GET);
            Routes("/blogs");
            // note: temporarily allow anonymous
            AllowAnonymous();
        }

        ExtendConfigure();
    }

    public override async Task HandleAsync(BlogQueryRequest req, CancellationToken ct)
    {
        var response = YieldResponse(_queryExecutor.ExecuteAsync(HttpContext.Request.Query, ct));

        // return no content

        await SendOkAsync(new Paginated<BlogResponse> { Data = response }, ct);

        async IAsyncEnumerable<BlogResponse> YieldResponse(IAsyncEnumerable<Blog> entities)
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
