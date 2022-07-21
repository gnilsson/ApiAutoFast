
using ApiAutoFast;
using ApiAutoFast.EntityFramework;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace ApiAutoFast.Sample.Server;

public partial class GetByIdAuthorEndpoint : EndpointBase<AuthorGetByIdRequest, AuthorResponse, AuthorMappingProfile>
{
    private readonly AutoFastSampleDbContext _dbContext;

    private static readonly string[] _relationalNavigationNames = new string[]
    {
        "Blogs",
    };

    public GetByIdAuthorEndpoint(AutoFastSampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        MapRoute("/authors/{id}", HttpVerb.Get);
        AllowAnonymous();
    }

    public override Task HandleAsync(AuthorGetByIdRequest req, CancellationToken ct)
    {
        return HandleRequestAsync(req, ct);
    }

    public override async Task HandleRequestAsync(AuthorGetByIdRequest req, CancellationToken ct)
    {
        var identifier = IdentifierUtility.ConvertFromRequest<Identifier>(req.Id, AddError);

        if (HasError())
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        var query = _dbContext.Authors.AsNoTracking();

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
