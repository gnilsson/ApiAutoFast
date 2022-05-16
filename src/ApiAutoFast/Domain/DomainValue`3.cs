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

    protected virtual string? MessageOnFailedValidation { get; }

    public TEntity EntityValue { get; private set; } = default!;

    protected virtual bool TryValidateRequestConversion(TRequest? requestValue, out TEntity entityValue)
    {
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

    public static implicit operator DomainValue<TRequest, TEntity, TResponse, TDomain>(TEntity entityValue)
    {
        var domain = _factory();
        domain.EntityValue = entityValue;
        return domain;
    }


    public override string ToString() => EntityValue!.ToString()!;
}


public class DomainValue<TRequest, TEntity, TDomain> : DomainValue<TRequest, TEntity, TRequest, TDomain>
    where TDomain : DomainValue<TRequest, TEntity, TRequest, TDomain>, new()
{
    protected override bool TryValidateRequestConversion(TRequest? requestValue, out TEntity entityValue)
    {
        throw new NotImplementedException($"{TypeText.DomainValue3} needs an overriden {nameof(TryValidateRequestConversion)} method.");
    }

    protected override TRequest ToResponse()
    {
        throw new NotImplementedException($"{TypeText.DomainValue3} needs an overriden {nameof(ToResponse)} method.");
    }
}

//public class DomainValue<TRequest, TEntity, TDomain> where TDomain : DomainValue<TRequest, TEntity, TDomain>, new()
//{
//    static DomainValue()
//    {
//        var ctor = typeof(TDomain)
//            .GetTypeInfo()
//            .DeclaredConstructors
//            .First();

//        var argsExp = Array.Empty<Expression>();
//        var newExp = Expression.New(ctor, argsExp);
//        var lambda = Expression.Lambda(typeof(Func<TDomain>), newExp);

//        _factory = (Func<TDomain>)lambda.Compile();
//    }

//    private static readonly Func<TDomain> _factory;

//    protected virtual string? MessageOnFailedValidation { get; }

//    public TEntity EntityValue { get; private set; } = default!;

//    protected virtual bool TryValidateRequestConversion(TRequest? requestValue, out TEntity entityValue)
//    {
//        throw new NotImplementedException($"DomainValue`3 needs an overriden {nameof(TryValidateRequestConversion)} method.");
//    }

//    //public static void Init()
//    //{
//    //    // note: invoked dynamically to trigger static ctor
//    //}

//    public static TDomain ConvertFromRequest(TRequest? request, Action<string, string> addError)
//    {
//        var domain = _factory();

//        if (domain.TryValidateRequestConversion(request, out var entityValue))
//        {
//            domain.EntityValue = entityValue;
//            return domain;
//        }

//        addError(typeof(TDomain).Name, domain.MessageOnFailedValidation ?? "Error when converting request.");
//        return default!;
//    }

//    public static implicit operator DomainValue<TRequest, TEntity, TDomain>(TEntity entityValue)
//    {
//        var domain = _factory();
//        domain.EntityValue = entityValue;
//        return domain;
//    }

//    public static implicit operator TEntity(DomainValue<TRequest, TEntity, TDomain> domain)
//    {
//        return domain.EntityValue;
//    }

//    //public static implicit operator TEntityRequestValue(DomainValue<TEntityRequestValue, TEntityRequestValue, TDomain> domain)
//    //{
//    //    return domain.EntityValue;
//    //}

//    public override string ToString() => EntityValue!.ToString()!;


//    // note: uncomment if this part is needed..

//    //public override int GetHashCode()
//    //{
//    //    return EqualityComparer<TEntityValue>.Default.GetHashCode(EntityValue!);
//    //}

//    //protected virtual bool Equals(DomainValue<TRequestValue, TEntityValue, TDomain> other)
//    //{
//    //    return EqualityComparer<TEntityValue>.Default.Equals(EntityValue, other.EntityValue);
//    //}

//    //public override bool Equals(object? obj)
//    //{
//    //    if (obj is null) return false;

//    //    if (ReferenceEquals(this, obj)) return true;

//    //    return obj.GetType() == GetType() && Equals((DomainValue<TRequestValue, TEntityValue, TDomain>)obj);
//    //}


//    //public static bool operator ==(DomainValue<TRequestValue, TEntityValue, TDomain> a, DomainValue<TRequestValue, TEntityValue, TDomain> b)
//    //{
//    //    if (a is null && b is null) return true;

//    //    if (a is null || b is null) return false;

//    //    return a.Equals(b);
//    //}

//    //public static bool operator !=(DomainValue<TRequestValue, TEntityValue, TDomain> a, DomainValue<TRequestValue, TEntityValue, TDomain> b)
//    //{
//    //    return !(a == b);
//    //}
//}
