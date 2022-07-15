using ApiAutoFast.SourceGenerator.Configuration;
using ApiAutoFast.SourceGenerator.Configuration.Enums;
using ApiAutoFast.SourceGenerator.Descriptive;
using System.Collections.Immutable;
using System.Text;

namespace ApiAutoFast.SourceGenerator.Emitters;

internal static class EndpointSourceEmitter
{
    private static Func<StringBuilder, EndpointConfig, StringBuilder> GetEndpointHandlerSource(EndpointTargetType endpointTargetType) => endpointTargetType switch
    {
        EndpointTargetType.Get => static (sb, endpointConfig) =>
        {
            sb.Append(@"
        var response = YieldResponse(_queryExecutor.ExecuteAsync(HttpContext.Request.Query, ct));

        await SendOkAsync(new Paginated<").Append(endpointConfig.Response).Append(@"> { Data = response }, ct);

        static async IAsyncEnumerable<").Append(endpointConfig.Response).Append(@"> YieldResponse(IAsyncEnumerable<").Append(endpointConfig.Entity).Append(@"> entities)
        {
            await foreach (var entity in entities)
            {
                yield return Map.FromEntity(entity);
            }
        }");
            return sb;
        }
        ,
        EndpointTargetType.Create => static (sb, endpointConfig) =>
        {
            sb.Append(@"
        var entity = Map.ToDomainEntity(req, AddError);

        if (HasError())
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        await _dbContext.AddAsync(entity, ct);

        if (ShouldSave())
        {
            await _dbContext.SaveChangesAsync(ct);
        }

        var response = Map.FromEntity(entity);

        await SendCreatedAtAsync<GetById").Append(endpointConfig.Entity).Append(@"Endpoint>(new { Id = entity.Id }, response, generateAbsoluteUrl: true, cancellation: ct);");
            return sb;
        }
        ,
        EndpointTargetType.Update => static (sb, endpointConfig) =>
        {
            sb.Append(@"
        var identifier = ").Append(TypeText.IdentifierUtility).Append(@".ConvertFromRequest<").Append(endpointConfig.IdType).Append(@">(req.Id, AddError);

        if (HasError())
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        var result = await _dbContext.").Append(endpointConfig.Entity).Append(@"s.FindAsync(new object?[] { identifier }, cancellationToken: ct);

        if (result is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var entity = Map.UpdateEntity(result, req);

        var response = Map.FromEntity(entity);

        await SendOkAsync(response, ct);");
            return sb;
        }
        ,
        EndpointTargetType.Delete => static (sb, endpointConfig) =>
        {
            sb.Append(@"
        var identifier = ").Append(TypeText.IdentifierUtility).Append(@".ConvertFromRequest<").Append(endpointConfig.IdType).Append(@">(req.Id, AddError);

        if (HasError())
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        var result = await _dbContext.").Append(endpointConfig.Entity).Append(@"s.FindAsync(new object?[] { identifier }, cancellationToken: ct);

        if (result is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        _dbContext.").Append(endpointConfig.Entity).Append(@"s.Remove(result);

        if (ShouldSave())
        {
            await _dbContext.SaveChangesAsync(ct);
        }

        await SendOkAsync(ct);");
            return sb;
        }
        ,
        EndpointTargetType.GetById => static (sb, endpointConfig) =>
        {
            sb.Append(@"
        var identifier = ").Append(TypeText.IdentifierUtility).Append(@".ConvertFromRequest<").Append(endpointConfig.IdType).Append(@">(req.Id, AddError);

        if (HasError())
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        var query = _dbContext.").Append(endpointConfig.Entity).Append(@"s.AsNoTracking();

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

        await SendOkAsync(response, ct);");
            return sb;
        }
        ,
        _ => static (sb, _) =>
        {
            sb.Append(@"
        await base.HandleAsync(req, ct);");
            return sb;
        }
    };

    private static Func<StringBuilder, EndpointConfig, StringBuilder> GetEndpointSetupSource(EndpointTargetType endpointTargetType) => endpointTargetType switch
    {
        EndpointTargetType.Get => static (sb, endpointConfig) =>
        {
            sb.Append(@"
    private static readonly Dictionary<string, Func<string, Expression<Func<").Append(endpointConfig.Entity).Append(@", bool>>>> _stringMethods = new()
    {");
            foreach (var propertyName in endpointConfig.StringEntityProperties)
            {
                sb.Append(@"
        [""").Append(propertyName).Append(@"""] = static query => entity => ((string)entity.").Append(propertyName).Append(@").Contains(query),");
            }
            sb.Append(@"
    };
    private static readonly string[] _relationalNavigationNames = new string[]
    {");
            foreach (var relationalNavigationName in endpointConfig.RelationalNavigationNames)
            {
                sb.Append(@"
        """).Append(relationalNavigationName).Append(@""",");
            }
            sb.Append(@"
    };
    private readonly IQueryExecutor<").Append(endpointConfig.Entity).Append(", ").Append(endpointConfig.IdType).Append(@"> _queryExecutor;

    public ").Append(endpointConfig.Endpoint).Append(@"(").Append(endpointConfig.ContextName).Append(@" dbContext)
    {
        _dbContext = dbContext;
        _queryExecutor = new QueryExecutor<").Append(endpointConfig.Entity).Append(", ").Append(endpointConfig.IdType).Append(@">(_dbContext.").Append(endpointConfig.Entity).Append(@"s, _stringMethods, _relationalNavigationNames);");
            sb.Append(@"
    }");
            return sb;
        }
        ,
        EndpointTargetType.GetById => static (sb, endpointConfig) =>
        {
            sb.Append(@"
    private static readonly string[] _relationalNavigationNames = new string[]
    {");
            foreach (var relationalNavigationName in endpointConfig.RelationalNavigationNames)
            {
                sb.Append(@"
        """).Append(relationalNavigationName).Append(@""",");
            }
            sb.Append(@"
    };

    public ").Append(endpointConfig.Endpoint).Append(@"(").Append(endpointConfig.ContextName).Append(@" dbContext)
    {
        _dbContext = dbContext;
    }");
            return sb;
        }
        ,
        EndpointTargetType.Create or EndpointTargetType.Update or EndpointTargetType.Delete => static (sb, endpointConfig) =>
        {
            sb.Append(@"
    public ").Append(endpointConfig.Endpoint).Append(@"(").Append(endpointConfig.ContextName).Append(@" dbContext)
    {
        _dbContext = dbContext;
    }");
            return sb;
        }
        ,
        _ => null!
    };

    internal static string EmitEndpoint(StringBuilder sb, string @namespace, EndpointConfig endpointConfig)
    {
        sb.Clear();

        sb.Append(@"
using ApiAutoFast;
using ApiAutoFast.EntityFramework;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace ").Append(@namespace).Append(@";
");
        sb.Append(@"
public ").Append(endpointConfig.IsTargetForGeneration ? "abstract" : "partial")
            .Append(" class ")
            .Append(endpointConfig.Endpoint)
            .Append(@" : EndpointBase<")
            .Append(endpointConfig.Request)
            .Append(@", ")
            .Append(endpointConfig.RequestEndpointPair.EndpointTarget is EndpointTargetType.Get ? $"Paginated<{endpointConfig.Response}>" : endpointConfig.Response)
            .Append(@", ")
            .Append(endpointConfig.MappingProfile)
            .Append(@">
{
    private readonly ").Append(endpointConfig.ContextName).Append(@" _dbContext;
");
        sb = GetEndpointSetupSource(endpointConfig.RequestEndpointPair.EndpointTarget)(sb, endpointConfig).Append(@"

    public override void Configure()
    {
        MapRoute(""").Append(endpointConfig.Route).Append(@""", ").Append(endpointConfig.RequestEndpointPair.HttpVerb).Append(@");
        AllowAnonymous();
    }
");
        if (endpointConfig.IsTargetForGeneration is false)
        {
            sb.Append(@"
    public override Task HandleAsync(").Append(endpointConfig.Request).Append(@" req, CancellationToken ct)
    {
        return HandleRequestAsync(req, ct);
    }
");
        }
        sb.Append(@"
    public override async Task HandleRequestAsync(").Append(endpointConfig.Request).Append(@" req, CancellationToken ct)
    {");
        sb = GetEndpointHandlerSource(endpointConfig.RequestEndpointPair.EndpointTarget)(sb, endpointConfig).Append(@"
    }
}
");
        return sb.ToString();
    }
}
