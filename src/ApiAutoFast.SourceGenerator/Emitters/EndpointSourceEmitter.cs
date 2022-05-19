using ApiAutoFast.SourceGenerator.Configuration;
using ApiAutoFast.SourceGenerator.Configuration.Enums;
using System.Text;

namespace ApiAutoFast.SourceGenerator.Emitters;

internal static class EndpointSourceEmitter
{
    private static readonly Func<EndpointTargetType, Func<StringBuilder, EndpointConfig, StringBuilder>> _endpointSourceGets = static (target) => target switch
    {
        EndpointTargetType.Get => static (sb, endpointConfig) =>
        {
            sb.Append(@"
        var result = await _dbContext.").Append(endpointConfig.EntityName).Append(@"s.Where(x => true).ToArrayAsync(ct);

        if (result.Length == 0)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var response = result.Select(x => Map.FromEntity(x));

        var pag = new ").Append(endpointConfig.Response).Append(@" { Data = response };

        await SendOkAsync(pag, ct);");
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

        await SendCreatedAtAsync<GetById").Append(endpointConfig.EntityName).Append(@"Endpoint>(new { Id = _entity.Id }, response, generateAbsoluteUrl: true, cancellation: ct);");
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

        var result = await _dbContext.").Append(endpointConfig.EntityName).Append(@"s.FindAsync(identifier);

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

        var result = await _dbContext.").Append(endpointConfig.EntityName).Append(@"s.FindAsync(identifier);

        if (result is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        _dbContext.").Append(endpointConfig.EntityName).Append(@"s.Remove(result);

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

        var result = await _dbContext.").Append(endpointConfig.EntityName).Append(@"s.FindAsync(identifier);

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

    internal static string EmitEndpoint(StringBuilder sb, string @namespace, EndpointConfig endpointConfig, string contextName)
    {
        sb.Clear();

        sb.Append(@"
using ApiAutoFast;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace ").Append(@namespace).Append(@";
");
        sb.Append(@"
public partial class ")
            .Append(endpointConfig.Name)
            .Append(@" : Endpoint<")
            .Append(endpointConfig.Request)
            .Append(@", ")
            .Append(endpointConfig.Response)
            .Append(@", ")
            .Append(endpointConfig.MappingProfile)
            .Append(@">
{
    partial void ExtendConfigure();
    private readonly ").Append(contextName).Append(@" _dbContext;
    private bool _overrideConfigure = false;");
        if (endpointConfig.RequestEndpointPair.EndpointTarget is EndpointTargetType.Create)
        {
            sb.Append(@"
    private bool _saveChanges = true;
    private ").Append(endpointConfig.EntityName).Append(@" _entity;");
        }
        sb.Append(@"

    public ").Append(endpointConfig.Name).Append(@"(").Append(contextName).Append(@" dbContext)
    {
        _dbContext = dbContext;
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
        sb = _endpointSourceGets(endpointConfig.RequestEndpointPair.EndpointTarget)(sb, endpointConfig).Append(@"
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
