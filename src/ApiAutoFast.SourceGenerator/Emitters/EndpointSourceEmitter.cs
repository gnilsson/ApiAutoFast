using ApiAutoFast.SourceGenerator.Configuration;
using ApiAutoFast.SourceGenerator.Configuration.Enums;
using System.Collections.Immutable;
using System.Text;

namespace ApiAutoFast.SourceGenerator.Emitters;

internal static class EndpointSourceEmitter
{
    private static Func<StringBuilder, EndpointConfig, StringBuilder> GetEndpointSource(EndpointTargetType endpointTargetType) => endpointTargetType switch
    {
        EndpointTargetType.Get => static (sb, endpointConfig) =>
        {
            sb.Append(@"
        var response = YieldResponse(_queryExecutor.ExecuteAsync(HttpContext.Request.Query, ct));

        // return no content

        await SendOkAsync(new Paginated<").Append(endpointConfig.Response).Append(@"> { Data = response }, ct);

        async IAsyncEnumerable<").Append(endpointConfig.Response).Append(@"> YieldResponse(IAsyncEnumerable<").Append(endpointConfig.Entity).Append(@"> entities)
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
        _entity = Map.ToDomainEntity(req, AddError);

        if (HasError())
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        await _dbContext.AddAsync(_entity, ct);

        if (_saveChanges)
        {
            await _dbContext.SaveChangesAsync(ct);
        }

        var response = Map.FromEntity(_entity);

        await SendCreatedAtAsync<GetById").Append(endpointConfig.Entity).Append(@"Endpoint>(new { Id = _entity.Id }, response, generateAbsoluteUrl: true, cancellation: ct);");
            return sb;
        }
        ,
        EndpointTargetType.Update => static (sb, endpointConfig) =>
        {
            sb.Append(@"
        var identifier = Identifier.ConvertFromRequest(req.Id, AddError);

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
        var identifier = Identifier.ConvertFromRequest(req.Id, AddError);

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

        await _dbContext.SaveChangesAsync(ct);

        await SendOkAsync(ct);");
            return sb;
        }
        ,
        EndpointTargetType.GetById => static (sb, endpointConfig) =>
        {
            sb.Append(@"
        var identifier = Identifier.ConvertFromRequest(req.Id, AddError);

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

    internal static string EmitEndpoint(StringBuilder sb, string @namespace, EndpointConfig endpointConfig, string contextName, ImmutableArray<string> relationalNavigationNames)
    {
        sb.Clear();

        var response = endpointConfig.RequestEndpointPair.EndpointTarget is EndpointTargetType.Get ? $"Paginated<{endpointConfig.Response}>" : endpointConfig.Response;

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
public partial class ")
            .Append(endpointConfig.Endpoint)
            .Append(@" : Endpoint<")
            .Append(endpointConfig.Request)
            .Append(@", ")
            .Append(response)
            .Append(@", ")
            .Append(endpointConfig.MappingProfile)
            .Append(@">
{
    partial void ExtendConfigure();
    private readonly ").Append(contextName).Append(@" _dbContext;
    private bool _overrideConfigure = false;
    private readonly QueryExecutor<").Append(endpointConfig.Entity).Append(@"> _queryExecutor;");
        if (endpointConfig.RequestEndpointPair.EndpointTarget is EndpointTargetType.Create)
        {
            sb.Append(@"
    private bool _saveChanges = true;
    private ").Append(endpointConfig.Entity).Append(@" _entity;");
        }
        else if (endpointConfig.RequestEndpointPair.EndpointTarget is EndpointTargetType.Get)
        {
            sb.Append(@"
    private static readonly Dictionary<string, Func<string, Expression<Func<").Append(endpointConfig.Entity).Append(@", bool>>>> _stringMethods = new()
    {");
            foreach (var propertyName in endpointConfig.StringEntityProperties)
            {
                sb.Append(@"
        [""").Append(propertyName).Append(@"""] = query => entity => ((string)entity.").Append(propertyName).Append(@").Contains(query),");
            }
            sb.Append(@"
    };");
        }

        if (endpointConfig.RequestEndpointPair.EndpointTarget is EndpointTargetType.Get or EndpointTargetType.GetById)
        {
            sb.Append(@"
    private static readonly string[] _relationalNavigationNames = new string[]
    {");
            foreach (var relationalNavigationName in relationalNavigationNames)
            {
                sb.Append(@"
        """).Append(relationalNavigationName).Append(@""",");
            }
            sb.Append(@"
    };");
        }
        sb.Append(@"


    public ").Append(endpointConfig.Endpoint).Append(@"(").Append(contextName).Append(@" dbContext)
    {
        _dbContext = dbContext;");
        if (endpointConfig.RequestEndpointPair.EndpointTarget is EndpointTargetType.Get)
        {
            sb.Append(@"
        _queryExecutor = new QueryExecutor<").Append(endpointConfig.Entity).Append(@">(_dbContext.").Append(endpointConfig.Entity).Append(@"s, _stringMethods, _relationalNavigationNames);");
        }
        sb.Append(@"
    }

    public override void Configure()
    {
        if (_overrideConfigure is false)
        {
            Verbs(").Append(endpointConfig.RequestEndpointPair.HttpVerb).Append(@");
            Routes(""").Append(endpointConfig.Route).Append(@""");
            // note: temporarily allow anonymous
            AllowAnonymous();
        }

        ExtendConfigure();
    }
");
        sb.Append(@"
    public override async Task HandleAsync(").Append(endpointConfig.Request).Append(@" req, CancellationToken ct)
    {");
        sb = GetEndpointSource(endpointConfig.RequestEndpointPair.EndpointTarget)(sb, endpointConfig).Append(@"
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
");
        return sb.ToString();
    }
}
