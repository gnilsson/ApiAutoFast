using ApiAutoFast.Descriptive;
using ApiAutoFast.Domain;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace ApiAutoFast;

// q: what about this?
public abstract class StringDomainValue<TDomain> : DomainValue<string, TDomain> where TDomain : DomainValue<string, TDomain>, new()
{
    public StringDomainValue()
    {
    }

    //public override StringDomainValue ConvertFromRequest(string? request, Action<string, string> addError)
    //{
    //    var a = base.ConvertFromRequest(request, addError);
    //}
    public bool Contains(string value)
    {
        return EntityValue!.Contains(value);
    }

    public static implicit operator string(StringDomainValue<TDomain> domain) => domain.EntityValue!;
}

public class DomainValue<TRequest, TEntity, TResponse, TDomain> where TDomain : DomainValue<TRequest, TEntity, TResponse, TDomain>, new()
{
    static DomainValue()
    {
        var ctor = typeof(TDomain)
            .GetTypeInfo()
            .DeclaredConstructors
            .First();

        var argsExp = Array.Empty<Expression>();
        var newExp = Expression.New(ctor, argsExp);
        var lambda = Expression.Lambda(typeof(Func<TDomain>), newExp);

        _factory = (Func<TDomain>)lambda.Compile();
    }

    private static readonly Func<TDomain> _factory;

    public TEntity? EntityValue { get; private set; }

    protected virtual bool TryValidateRequestConversion(TRequest? requestValue, [NotNullWhen(true)] out TEntity entityValue)
    {
        entityValue = requestValue is TEntity entityRequestValue ? entityRequestValue : default!;

        return requestValue is not null;
    }

    public virtual TResponse ToResponse()
    {
        throw new NotImplementedException($"{TypeText.DomainValue4} needs an overriden {nameof(ToResponse)} method.");
    }

    public static TDomain? ConvertFromRequest(TRequest? request, Action<string, string> addError)
    {
        var domain = _factory();

        if (domain.TryValidateRequestConversion(request, out var entityValue))
        {
            domain.EntityValue = entityValue;
            return domain;
        }

        addError(typeof(TDomain).Name, ValidationErrorMessageContainer.Get<TDomain>());
        return default!;
    }

    public static TDomain? UpdateFromRequest(TDomain domain, TRequest? request, Action<string, string> addError)
    {
        if (request is null)
        {
            return domain;
        }

        if (domain.TryValidateRequestConversion(request, out var entityValue))
        {
            return (TDomain)entityValue;
        }

        addError(typeof(TDomain).Name, ValidationErrorMessageContainer.Get<TDomain>() );
        return default!;
    }

    public static implicit operator DomainValue<TRequest, TEntity, TResponse, TDomain>(TEntity entityValue) => From(entityValue);

    public static implicit operator TEntity(DomainValue<TRequest, TEntity, TResponse, TDomain> domain) => domain.EntityValue!;

    public static DomainValue<TRequest, TEntity, TResponse, TDomain> From(TEntity entityValue)
    {
        var domain = _factory();
        domain.EntityValue = entityValue;
        return domain;
    }

    public override string ToString() => EntityValue!.ToString()!;
}
