////////using ApiAutoFast.EntityFramework;
////////using FastEndpoints;
////////using FluentValidation.Results;
////////using Microsoft.AspNetCore.Authentication.Cookies;
////////using System.Linq.Expressions;

////////namespace ApiAutoFast.Sample.Server;

////////public partial class CreateBlogEndpoint : Endpoint<BlogCreateCommand, BlogResponse, BlogMappingProfile>
////////{
////////    partial
////////    public override void Configure()
////////    {
////////        base.Configure();
////////    }
////////}


////////public partial class CreateBlogEndpoint : Endpoint<BlogCreateCommand, BlogResponse, BlogMappingProfile>
////////{


////////    partial void ExtendConfigure()
////////    {
////////        base.AuthSchemes(new[] { "jwt" });
////////        base.Policies(new[] { "isAdmin" });
////////        //base.ResponseStarted
////////    }

////////    void ExtendHandleAsync()
////////    {

////////    }

////////}


//using ApiAutoFast;
//using ApiAutoFast.EntityFramework;
//using FastEndpoints;
//using FluentValidation.Results;
//using Microsoft.EntityFrameworkCore;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Linq.Expressions;

//namespace ApiAutoFast.Sample.Server;


//public interface IExtendHandle<TRequest>
//{
//    Task ExtendedHandleAsync(TRequest request, CancellationToken cancellationToken);
//}

//// [AutoFastEndpoint(typeof(Blog), EndpointVerb.Get)]
//public class GetBlogEndpointChild : GetBlogEndpoint2
//{
//    public GetBlogEndpointChild(AutoFastSampleDbContext dbContext) : base(dbContext)
//    {
//    }


//    public override void Configure()
//    {
//        Verbs(Http.GET);
//        Routes("/blogs2");
//        // note: temporarily allow anonymous
//        AllowAnonymous();
//    }

//    //public override void ConfigureAutoFast()
//    //{

//    //}

//    public override async Task HandleAsync(BlogQueryRequest req, CancellationToken ct)
//    {
//        await _handleAsync(req, ct);
//    }

//}

////public class FakeBlog : IEntity<Identifier>
////{
////    public ApiAutoFast.Title Tit { get; set; }
////    public DateTime CreatedDateTime { get; set; }
////    public DateTime ModifiedDateTime { get; set; }
////    public Identifier Id { get; set; }
////}

//public partial class BlogMappingProfile2 : Mapper<BlogCreateCommand, BlogResponse, Blog>
//{
//    private readonly bool _onOverrideUpdateEntity = false;

//    partial void OnOverrideUpdateEntity(ref Blog originalEntity, BlogModifyCommand e);

//    public override BlogResponse FromEntity(Blog e)
//    {
//        return e.AdaptToResponse();
//    }

//    public Blog UpdateEntity(Blog originalEntity, BlogModifyCommand e)
//    {
//        if (_onOverrideUpdateEntity)
//        {
//            OnOverrideUpdateEntity(ref originalEntity, e);
//            return originalEntity;
//        }

//        return originalEntity;
//    }

//    public Blog ToDomainEntity(BlogCreateCommand command, Action<string, string> addValidationError)
//    {
//        return new Blog
//        {
//            Title = Title2.ConvertFromRequest<Title2>(command.Title, addValidationError),
//        };
//    }
//}


//public abstract class GetBlogEndpoint2 : Endpoint<BlogQueryRequest, Paginated<BlogResponse>, BlogMappingProfile>, IExtendHandle<BlogQueryRequest>
//{
//    // partial void ExtendConfigure();
//    private readonly AutoFastSampleDbContext _dbContext;
//    private bool _overrideConfigure = false;
//    private readonly QueryExecutor<Blog> _queryExecutor;
//    private static readonly Dictionary<string, Func<string, Expression<Func<Blog, bool>>>> _stringMethods = new()
//    {
//        ["Title"] = static query => entity => entity.Title.Contains(query),
//    };
//    private static readonly string[] _relationalNavigationNames = new string[]
//    {
//        "Posts",
//    };


//    public GetBlogEndpoint2(AutoFastSampleDbContext dbContext)
//    {
//        _dbContext = dbContext;
//        _queryExecutor = new QueryExecutor<Blog>(_dbContext.Blogs, _stringMethods, _relationalNavigationNames);
//        _handleAsync = HandleRequestAsync;
//    }

//    //public override void Configure()
//    //{
//    //    if (_overrideConfigure is false)
//    //    {
//    //        Verbs(Http.GET);
//    //        Routes("/blogs2");
//    //        // note: temporarily allow anonymous
//    //        AllowAnonymous();
//    //    }

//    //    ExtendConfigure();
//    //}

//    protected readonly Func<BlogQueryRequest, CancellationToken, Task> _handleAsync;

//    public override async Task HandleAsync(BlogQueryRequest req, CancellationToken ct)
//    {
//        await _handleAsync(req, ct);
//        //return HandleRequestAsync(null!, ct);
//    }

//    private async Task HandleRequestAsync(BlogQueryRequest _, CancellationToken ct)
//    {
//        var response = YieldResponse(_queryExecutor.ExecuteAsync(HttpContext.Request.Query, ct));

//        // return no content

//        await SendOkAsync(new Paginated<BlogResponse> { Data = response }, ct);

//        static async IAsyncEnumerable<BlogResponse> YieldResponse(IAsyncEnumerable<Blog> entities)
//        {
//            await foreach (var entity in entities)
//            {
//                yield return Map.FromEntity(entity);
//            }
//        }
//    }

//    private void AddError(string property, string message)
//    {
//        ValidationFailures.Add(new ValidationFailure(property, message));
//    }

//    private bool HasError()
//    {
//        return ValidationFailures.Count > 0;
//    }

//    public async Task ExtendedHandleAsync(BlogQueryRequest request, CancellationToken cancellationToken)
//    {
//        await _handleAsync(request, cancellationToken);
//    }
//}
