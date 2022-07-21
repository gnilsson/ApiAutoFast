
using ApiAutoFast;
using ApiAutoFast.EntityFramework;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace ApiAutoFast.Sample.Server;

public partial class GetAuthorEndpoint : EndpointBase<AuthorQueryRequest, Paginated<AuthorResponse>, AuthorMappingProfile>
{
    private readonly AutoFastSampleDbContext _dbContext;

    private static readonly Dictionary<string, Func<string, Expression<Func<Author, bool>>>> _stringMethods = new()
    {
        ["FirstName"] = static query => entity => ((string)entity.FirstName).Contains(query),
        ["LastName"] = static query => entity => ((string)entity.LastName).Contains(query),
    };
    private static readonly string[] _relationalNavigationNames = new string[]
    {
        "Blogs",
    };
    private readonly IQueryExecutor<Author, Identifier> _queryExecutor;

    public GetAuthorEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
        _queryExecutor = new QueryExecutor<Author, Identifier>(_dbContext.Authors, _stringMethods, _relationalNavigationNames);
    }

    public override void Configure()
    {
        MapRoute("/authors", HttpVerb.Get);
        AllowAnonymous();
    }

    public override Task HandleAsync(AuthorQueryRequest req, CancellationToken ct)
    {
        return HandleRequestAsync(req, ct);
    }

    public override async Task HandleRequestAsync(AuthorQueryRequest req, CancellationToken ct)
    {
        var response = YieldResponse(_queryExecutor.ExecuteAsync(HttpContext.Request.Query, ct));

        await SendOkAsync(new Paginated<AuthorResponse> { Data = response }, ct);

        static async IAsyncEnumerable<AuthorResponse> YieldResponse(IAsyncEnumerable<Author> entities)
        {
            await foreach (var entity in entities)
            {
                yield return Map.FromEntity(entity);
            }
        }
    }
}
