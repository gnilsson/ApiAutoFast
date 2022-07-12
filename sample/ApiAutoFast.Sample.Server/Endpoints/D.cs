
////using ApiAutoFast;
////using ApiAutoFast.EntityFramework;
////using FastEndpoints;
////using FluentValidation.Results;
////using Microsoft.EntityFrameworkCore;
////using System.Threading;
////using System.Threading.Tasks;
////using System.Linq.Expressions;

////namespace ApiAutoFast.Sample.Server;

////public partial class CreateBlogEndpoint3 : EndpointBase<BlogCreateCommand, BlogResponse, BlogMappingProfile>
////{
////    private readonly AutoFastSampleDbContext _dbContext;
////    private bool _saveChanges = true;


////    public CreateBlogEndpoint3(AutoFastSampleDbContext dbContext)
////    {
////        _dbContext = dbContext;
////    }

////    public override void Configure()
////    {
////        Verbs(Http.POST);
////        Routes("/blogs2");
////        AllowAnonymous();
////    }

////    public override Task HandleAsync(BlogCreateCommand req, CancellationToken ct)
////    {
////        return HandleRequestAsync(req, ct);
////    }

////    public override async Task HandleRequestAsync(BlogCreateCommand request, CancellationToken ct)
////    {
////        var entity = Map.ToDomainEntity(request, AddError);

////        if (HasError())
////        {
////            await SendErrorsAsync(400, ct);
////            return;
////        }

////        await _dbContext.AddAsync(entity, ct);

////        if (_saveChanges)
////        {
////            await _dbContext.SaveChangesAsync(ct);
////        }

////        var response = Map.FromEntity(entity);

////        await SendCreatedAtAsync<GetByIdBlogEndpoint>(new { Id = entity.Id }, response, generateAbsoluteUrl: true, cancellation: ct);
////    }
////}

////public class CreateBlogEndpointExt : CreateBlogEndpoint4
////{
////    public CreateBlogEndpointExt(AutoFastSampleDbContext dbContext) : base(dbContext)
////    {
////    }

////    public override void Configure()
////    {
////        base.Configure();
////        AllowAnonymous();
////    }

////    public override void ConfigureAutoFast()
////    {
////        SkipSaveChanges();
////    }
////}

////public abstract class CreateBlogEndpoint4 : EndpointBase<BlogCreateCommand, BlogResponse, BlogMappingProfile>
////{
////    private readonly AutoFastSampleDbContext _dbContext;

////    internal bool SaveChanges { get; set; }

////    public CreateBlogEndpoint4(AutoFastSampleDbContext dbContext)
////    {
////        _dbContext = dbContext;
////    }

////    public override void Configure()
////    {
////        Verbs(Http.POST);
////        Routes("/blogs4");
////        AllowAnonymous();
////    }

////    public override async Task HandleRequestAsync(BlogCreateCommand request, CancellationToken ct)
////    {
////        var entity = Map.ToDomainEntity(request, AddError);

////        if (HasError())
////        {
////            await SendErrorsAsync(400, ct);
////            return;
////        }

////        await _dbContext.AddAsync(entity, ct);

////        if (ShouldSave())
////        {
////            await _dbContext.SaveChangesAsync(ct);
////        }

////        var response = Map.FromEntity(entity);

////        await SendCreatedAtAsync<GetByIdBlogEndpoint>(new { Id = entity.Id }, response, generateAbsoluteUrl: true, cancellation: ct);
////    }
////}



//using FastEndpoints;

//namespace ApiAutoFast.Sample.Server;

//[AutoFastEndpoint]
//public class CreateBlogEndpointExt : CreateBlogEndpoint5
//{
//    public CreateBlogEndpointExt(AutoFastSampleDbContext dbContext) : base(dbContext)
//    {
//    }

//    public override void Configure()
//    {
//        base.Configure();
//    }
//}

//public abstract class CreateBlogEndpoint5 : EndpointBase<BlogCreateCommand, BlogResponse, BlogMappingProfile>
//{
//    private readonly AutoFastSampleDbContext _dbContext;

//    public CreateBlogEndpoint5(AutoFastSampleDbContext dbContext)
//    {
//        _dbContext = dbContext;
//    }

//    public override void Configure()
//    {
//        MapRoute("/blogs", HttpVerb.Post);
//        AllowAnonymous();
//    }

//    public override async Task HandleRequestAsync(BlogCreateCommand req, CancellationToken ct)
//    {
//        var entity = Map.ToDomainEntity(req, AddError);

//        if (HasError())
//        {
//            await SendErrorsAsync(400, ct);
//            return;
//        }

//        await _dbContext.AddAsync(entity, ct);

//        if (ShouldSave())
//        {
//            await _dbContext.SaveChangesAsync(ct);
//        }

//        var response = Map.FromEntity(entity);

//        await SendCreatedAtAsync<GetByIdBlogEndpoint>(new { Id = entity.Id }, response, generateAbsoluteUrl: true, cancellation: ct);
//    }
//}

////HintName: GetBlogEndpoint.g.cs

//using ApiAutoFast;
//using ApiAutoFast.EntityFramework;
//using FastEndpoints;
//using FluentValidation.Results;
//using Microsoft.EntityFrameworkCore;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Linq.Expressions;
//using System.Reflection.Metadata;

//namespace ApiAutoFast.Sample.Server;

//public partial class GetBlogEndpoint : EndpointBase<BlogQueryRequest, Paginated<BlogResponse>, BlogMappingProfile>
//{
//    private readonly AutoFastSampleDbContext _dbContext;

//    private static readonly Dictionary<string, Func<string, Expression<Func<Blog, bool>>>> _stringMethods = new()
//    {
//        ["Title"] = static query => entity => ((string)entity.Title).Contains(query),
//    };
//    private static readonly string[] _relationalNavigationNames = new string[]
//{
//};
//    private readonly IQueryExecutor<Blog> _queryExecutor;

//    public GetBlogEndpoint(AutoFastSampleDbContext dbContext)
//    {
//        _dbContext = dbContext;
//        _queryExecutor = new QueryExecutor<Blog>(_dbContext.Blogs, _stringMethods, _relationalNavigationNames);
//    }

//    public override void Configure()
//    {
//        MapRoute("/blogs", HttpVerb.Get);
//        AllowAnonymous();
//    }

//    public override Task HandleAsync(BlogQueryRequest req, CancellationToken ct)
//    {
//        return HandleRequestAsync(req, ct);
//    }

//    public override async Task HandleRequestAsync(BlogQueryRequest req, CancellationToken ct)
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
