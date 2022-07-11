//using ApiAutoFast.EntityFramework;
//using FastEndpoints;
//using FluentValidation.Results;
//using Microsoft.EntityFrameworkCore;
//using System.Linq.Expressions;
//using System.Reflection.Metadata;

//namespace ApiAutoFast.Sample.Server;

//public class GetBlogExtended : GetBlogEndpoint2
//{
//    public GetBlogExtended(AutoFastSampleDbContext dbContext) : base(dbContext)
//    {

//    }

//    public override async Task HandleAsync(BlogQueryRequest req, CancellationToken ct)
//    {
//        await _handleRequestAsync(req, ct);
//        // return response?
//    }

//    public override void Configure()
//    {
//        base.Configure();
//        AllowAnonymous();
//    }

//}

//public abstract class GetBlogEndpoint2 : EndpointBase<BlogQueryRequest, Paginated<BlogResponse>, BlogMappingProfile>
//{
//    private static readonly Dictionary<string, Func<string, Expression<Func<Blog, bool>>>> _stringMethods = new()
//    {
//        ["Title"] = static query => entity => ((string)entity.Title).Contains(query),
//    };
//    private static readonly string[] _relationalNavigationNames = new string[]
//    {
//    };
//    private readonly IQueryExecutor<Blog> _queryExecutor;
//    private readonly AutoFastSampleDbContext _dbContext;

//    public GetBlogEndpoint2(AutoFastSampleDbContext dbContext)
//    {
//        _dbContext = dbContext;
//        _queryExecutor = new QueryExecutor<Blog>(_dbContext.Blogs, _stringMethods, _relationalNavigationNames);
//    }

//    public override void Configure()
//    {
//        Verbs(Http.GET);
//        Routes("/blogs2");
//        //AllowAnonymous();
//    }

//    public override async Task HandleRequestAsync(BlogQueryRequest request, CancellationToken ct)
//    {
//        var response = YieldResponse(_queryExecutor.ExecuteAsync(HttpContext.Request.Query, ct));

//        await SendOkAsync(new Paginated<BlogResponse> { Data = response }, ct);

//        static async IAsyncEnumerable<BlogResponse> YieldResponse(IAsyncEnumerable<Blog> entities)
//        {
//            await foreach (var entity in entities)
//            {
//                yield return Map.FromEntity(entity);
//            }
//        }
//    }
//}


//public partial class GetBlogEndpoint3 : EndpointBase<BlogQueryRequest, Paginated<BlogResponse>, BlogMappingProfile>
//{
//    private static readonly Dictionary<string, Func<string, Expression<Func<Blog, bool>>>> _stringMethods = new()
//    {
//        ["Title"] = static query => entity => ((string)entity.Title).Contains(query),
//    };
//    private readonly AutoFastSampleDbContext _dbContext;
//    private readonly IQueryExecutor<Blog> _queryExecutor;
//    private bool _saveChanges = true;
//    private static readonly string[] _relationalNavigationNames = new string[]
//    {
//    };

//    public GetBlogEndpoint3(AutoFastSampleDbContext dbContext)
//    {
//        _dbContext = dbContext;
//        _queryExecutor = new QueryExecutor<Blog>(_dbContext.Blogs, _stringMethods, _relationalNavigationNames);
//    }

//    public override void Configure()
//    {
//        Verbs(Http.GET);
//        Routes("/blogs3");
//        AllowAnonymous();
//    }

//    public override Task HandleAsync(BlogQueryRequest req, CancellationToken ct)
//    {
//        return HandleRequestAsync(req, ct);
//    }

//    public override async Task HandleRequestAsync(BlogQueryRequest request, CancellationToken ct)
//    {
//        var response = YieldResponse(_queryExecutor.ExecuteAsync(HttpContext.Request.Query, ct));

//        await SendOkAsync(new Paginated<BlogResponse> { Data = response }, ct);

//        static async IAsyncEnumerable<BlogResponse> YieldResponse(IAsyncEnumerable<Blog> entities)
//        {
//            await foreach (var entity in entities)
//            {
//                yield return Map.FromEntity(entity);
//            }
//        }
//    }
//}

