
//using ApiAutoFast;
//using ApiAutoFast.EntityFramework;
//using FastEndpoints;
//using FluentValidation.Results;
//using Microsoft.EntityFrameworkCore;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Linq.Expressions;

//namespace ApiAutoFast.Sample.Server;

//public partial class GetByIdBlogEndpoint2 : EndpointBase<BlogGetByIdRequest, BlogResponse, BlogMappingProfile>
//{
//    private readonly AutoFastSampleDbContext _dbContext;

//    private static readonly string[] _relationalNavigationNames = new string[]
//    {
//    };

//    public GetByIdBlogEndpoint2(AutoFastSampleDbContext dbContext)
//    {
//        _dbContext = dbContext;
//    }

//    public override void Configure()
//    {
//        MapRoute("/blogs/{id}", HttpVerb.Get);
//        AllowAnonymous();
//    }

//    public override Task HandleAsync(BlogGetByIdRequest req, CancellationToken ct)
//    {
//        return HandleRequestAsync(req, ct);
//    }

//    public override async Task HandleRequestAsync(BlogGetByIdRequest req, CancellationToken ct)
//    {
//        var identifier = Identifier.ConvertFromRequest(req.Id, AddError);

//        if (HasError())
//        {
//            await SendErrorsAsync(400, ct);
//            return;
//        }

//        var query = _dbContext.Blogs.AsNoTracking();

//        foreach (var relationalNavigationName in _relationalNavigationNames)
//        {
//            query = query.Include(relationalNavigationName);
//        }

//        var result = await query.SingleOrDefaultAsync(x => x.Id == identifier, ct);

//        if (result is null)
//        {
//            await SendNotFoundAsync(ct);
//            return;
//        }

//        var response = Map.FromEntity(result);

//        await SendOkAsync(response, ct);
//    }
//}
