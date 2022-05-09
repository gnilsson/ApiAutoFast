//HintName: CreateBlogEndpoint.g.cs

using ApiAutoFast;
using FastEndpoints;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ApiAutoFast.Sample.Server.Database;

public partial class CreateBlogEndpoint : Endpoint<BlogCreateCommand, BlogResponse, BlogMappingProfile>
{
    partial void OnExtendConfigure();
    private bool _extendConfigure = false;
    private readonly AutoFastSampleDbContext _dbContext;

    public CreateBlogEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        if (_extendConfigure is false)
        {
            Verbs(Http.POST);
            Routes("/blogs");
            AllowAnonymous();
        }

        OnExtendConfigure();
    }

    public override async Task HandleAsync(BlogCreateCommand req, CancellationToken ct)
    {
        var entity = Map.ToEntity(req);

        await _dbContext.AddAsync(entity, ct);

        await _dbContext.SaveChangesAsync(ct);

        var response = Map.FromEntity(entity);

        await SendCreatedAtAsync<GetByIdBlogEndpoint>(response.Id, response, Http.GET, cancellation: ct);
    }
}
