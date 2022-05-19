using ApiAutoFast.Descriptive;
using System.Linq.Expressions;
using System.Reflection;

namespace ApiAutoFast;

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

    // todo: add this message to some static dictionary that is setup via config
    protected virtual string? MessageOnFailedValidation { get; }

    public TEntity EntityValue { get; private set; } = default!;

    protected virtual bool TryValidateRequestConversion(TRequest? requestValue, out TEntity entityValue)
    {
        if (requestValue is TEntity entityRequestValue)
        {
            entityValue = entityRequestValue;
            return requestValue is not null;
        }

        throw new NotImplementedException($"{TypeText.DomainValue4} needs an overriden {nameof(TryValidateRequestConversion)} method.");
    }

    protected virtual TResponse ToResponse()
    {
        throw new NotImplementedException($"{TypeText.DomainValue4} needs an overriden {nameof(ToResponse)} method.");
    }

    //public static void Init()
    //{
    //    // note: invoked dynamically to trigger static ctor
    //}

    public static TDomain ConvertFromRequest(TRequest? request, Action<string, string> addError)
    {
        var domain = _factory();

        if (domain.TryValidateRequestConversion(request, out var entityValue))
        {
            domain.EntityValue = entityValue;
            return domain;
        }

        addError(typeof(TDomain).Name, domain.MessageOnFailedValidation ?? "Error when converting request.");
        return default!;
    }

    public static implicit operator DomainValue<TRequest, TEntity, TResponse, TDomain>(TEntity entityValue) => From(entityValue);
    //public static implicit operator DomainValue<TRequest, TDomain>(TEntity entityValue) => From(entityValue);

    public static DomainValue<TRequest, TEntity, TResponse, TDomain> From(TEntity entityValue)
    {
        var domain = _factory();
        domain.EntityValue = entityValue;
        return domain;
    }

    //public static implicit operator TDomain(DomainValue<TRequest, TEntity, TResponse, TDomain> domain) => domain;
    public static implicit operator TEntity(DomainValue<TRequest, TEntity, TResponse, TDomain> domain) => domain.EntityValue;

    public override string ToString() => EntityValue!.ToString()!;
}
