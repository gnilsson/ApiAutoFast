
//using ApiAutoFast;
//using ApiAutoFast.EntityFramework;
//using FastEndpoints;
//using FluentValidation.Results;
//using Microsoft.EntityFrameworkCore;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Linq.Expressions;

//namespace ApiAutoFast.Sample.Server;

//public partial class CreateBlogEndpoint3 : EndpointBase<BlogCreateCommand, BlogResponse, BlogMappingProfile>
//{
//    private readonly AutoFastSampleDbContext _dbContext;
//    private bool _saveChanges = true;


//    public CreateBlogEndpoint3(AutoFastSampleDbContext dbContext)
//    {
//        _dbContext = dbContext;
//    }

//    public override void Configure()
//    {
//        Verbs(Http.POST);
//        Routes("/blogs2");
//        AllowAnonymous();
//    }

//    public override Task HandleAsync(BlogCreateCommand req, CancellationToken ct)
//    {
//        return HandleRequestAsync(req, ct);
//    }

//    public override async Task HandleRequestAsync(BlogCreateCommand request, CancellationToken ct)
//    {
//        var entity = Map.ToDomainEntity(request, AddError);

//        if (HasError())
//        {
//            await SendErrorsAsync(400, ct);
//            return;
//        }

//        await _dbContext.AddAsync(entity, ct);

//        if (_saveChanges)
//        {
//            await _dbContext.SaveChangesAsync(ct);
//        }

//        var response = Map.FromEntity(entity);

//        await SendCreatedAtAsync<GetByIdBlogEndpoint>(new { Id = entity.Id }, response, generateAbsoluteUrl: true, cancellation: ct);
//    }
//}

//public class CreateBlogEndpointExt : CreateBlogEndpoint4
//{
//    public CreateBlogEndpointExt(AutoFastSampleDbContext dbContext) : base(dbContext)
//    {
//    }

//    public override void Configure()
//    {
//        base.Configure();
//        AllowAnonymous();
//    }

//    public override void ConfigureAutoFast()
//    {
//        SkipSaveChanges();
//    }
//}

//public abstract class CreateBlogEndpoint4 : EndpointBase<BlogCreateCommand, BlogResponse, BlogMappingProfile>
//{
//    private readonly AutoFastSampleDbContext _dbContext;

//    internal bool SaveChanges { get; set; }

//    public CreateBlogEndpoint4(AutoFastSampleDbContext dbContext)
//    {
//        _dbContext = dbContext;
//    }

//    public override void Configure()
//    {
//        Verbs(Http.POST);
//        Routes("/blogs4");
//        AllowAnonymous();
//    }

//    public override async Task HandleRequestAsync(BlogCreateCommand request, CancellationToken ct)
//    {
//        var entity = Map.ToDomainEntity(request, AddError);

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
