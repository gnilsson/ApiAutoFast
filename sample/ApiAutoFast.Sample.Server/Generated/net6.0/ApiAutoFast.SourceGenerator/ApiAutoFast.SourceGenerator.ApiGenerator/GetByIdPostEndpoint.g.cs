﻿
#nullable enable

using ApiAutoFast;
using ApiAutoFast.EntityFramework;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace ApiAutoFast.Sample.Server;

public partial class GetByIdPostEndpoint : EndpointBase<PostGetByIdRequest, PostResponse, PostMappingProfile>
{
    private readonly AutoFastSampleDbContext _dbContext;

    private static readonly string[] _relationalNavigationNames = new string[]
    {
        "Blog",
    };

    public GetByIdPostEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        MapRoute("/posts/{id}", HttpVerb.Get);
        AllowAnonymous();
    }

    public override Task HandleAsync(PostGetByIdRequest req, CancellationToken ct)
    {
        return HandleRequestAsync(req, ct);
    }

    public override async Task HandleRequestAsync(PostGetByIdRequest req, CancellationToken ct)
    {
        var identifier = IdentifierUtility.ConvertFromRequest<Identifier>(req.Id, AddError);

        if (HasError())
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        var query = _dbContext.Posts.AsNoTracking();

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
