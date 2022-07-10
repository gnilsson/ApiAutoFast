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

public partial class GetByIdBlogEndpoint : Endpoint<BlogGetByIdRequest, BlogResponse, BlogMappingProfile>
{
    partial void ExtendConfigure();
    private readonly AutoFastSampleDbContext _dbContext;
    private readonly IQueryExecutor<Blog> _queryExecutor;
    private bool _overrideConfigure = false;
    private bool _saveChanges = true;
    private bool _terminateHandler = false;
    private static readonly string[] _relationalNavigationNames = new string[]
    {
    };


    public GetByIdBlogEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        if (_overrideConfigure is false)
        {
            Verbs(Http.GET);
            Routes("/blogs/{id}");
            // note: temporarily allow anonymous
            AllowAnonymous();
        }

        ExtendConfigure();
    }

    public override async Task HandleAsync(BlogGetByIdRequest req, CancellationToken ct)
    {
        if(_terminateHandler) return;

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

    private void AddError(string property, string message)
    {
        ValidationFailures.Add(new ValidationFailure(property, message));
    }

    private bool HasError()
    {
        return ValidationFailures.Count > 0;
    }
}
