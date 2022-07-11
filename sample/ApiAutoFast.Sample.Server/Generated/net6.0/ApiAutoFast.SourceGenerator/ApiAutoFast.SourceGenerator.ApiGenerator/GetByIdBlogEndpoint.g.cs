﻿
using ApiAutoFast;
using ApiAutoFast.EntityFramework;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace ApiAutoFast.Sample.Server;

public abstract class GetByIdBlogEndpoint : EndpointBase<BlogGetByIdRequest, BlogResponse, BlogMappingProfile>
{
    private readonly AutoFastSampleDbContext _dbContext;

    private static readonly string[] _relationalNavigationNames = new string[]
    {
    };

    public GetByIdBlogEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Verbs(Http.GET);
        Routes("/blogs/{id}");
        AllowAnonymous();
    }

    public override async Task HandleRequestAsync(BlogGetByIdRequest req, CancellationToken ct)
    {

        var identifier = Identifier.ConvertFromRequest(req.Id, AddError);

        if (HasError())
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        var query = _dbContext.Blogs.AsNoTracking();

        foreach (var relationalNavigationName in _relationalNavigationNames)
        {
            query = query.Include(relationalNavigationName);
        }

        var result = await query.SingleOrDefaultAsync(x => x.Id == identifier, ct);

        if (result is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var response = Map.FromEntity(result);

        await SendOkAsync(response, ct);
    }
}
