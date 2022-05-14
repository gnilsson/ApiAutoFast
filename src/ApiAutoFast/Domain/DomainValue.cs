﻿using System.Linq.Expressions;
using System.Reflection;

namespace ApiAutoFast;

public class DomainValue<TRequestValue, TEntityValue, TDomain>
    where TDomain : DomainValue<TRequestValue, TEntityValue, TDomain>, new()
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

        // if is enum add to container
    }

    public static void Init()
    {
        // note: invoked dynamically to trigger static ctor
    }

    private static readonly Func<TDomain> _factory;

    public TEntityValue EntityValue { get; internal set; } = default!;

    protected virtual string? MessageOnFailedValidation { get; }

    protected virtual bool TryValidateRequestConversion(TRequestValue? requestValue, out TEntityValue entityValue)
    {
        throw new NotImplementedException($"DomainValue`3 needs an overriden {nameof(TryValidateRequestConversion)} method.");
    }

    public static TDomain ConvertFromRequest(TRequestValue? request, Action<string, string> addError)
    {
        var domain = _factory();

        if (domain.TryValidateRequestConversion(request, out var entityValue))
        {
            domain.EntityValue = entityValue;
            return domain;
        }

        addError(nameof(TDomain), domain.MessageOnFailedValidation ?? "Error when converting request.");
        return default!;
    }

    public static implicit operator DomainValue<TRequestValue, TEntityValue, TDomain>(TEntityValue entityValue)
    {
        var domain = _factory();
        domain.EntityValue = entityValue;
        return domain;
    }

    protected virtual bool Equals(DomainValue<TRequestValue, TEntityValue, TDomain> other)
    {
        return EqualityComparer<TEntityValue>.Default.Equals(EntityValue, other.EntityValue);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        if (ReferenceEquals(this, obj)) return true;

        return obj.GetType() == GetType() && Equals((DomainValue<TRequestValue, TEntityValue, TDomain>)obj);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<TEntityValue>.Default.GetHashCode(EntityValue!);
    }

    public static bool operator ==(DomainValue<TRequestValue, TEntityValue, TDomain> a, DomainValue<TRequestValue, TEntityValue, TDomain> b)
    {
        if (a is null && b is null) return true;

        if (a is null || b is null) return false;

        return a.Equals(b);
    }

    public static bool operator !=(DomainValue<TRequestValue, TEntityValue, TDomain> a, DomainValue<TRequestValue, TEntityValue, TDomain> b)
    {
        return !(a == b);
    }

    public override string ToString() => EntityValue!.ToString()!;
}

public class DomainValue<TEntityRequestValue, TThis> : DomainValue<TEntityRequestValue, TEntityRequestValue, TThis>
    where TThis : DomainValue<TEntityRequestValue, TEntityRequestValue, TThis>, new()
{
    // note: can something be done here?
    protected override bool TryValidateRequestConversion(TEntityRequestValue? requestValue, out TEntityRequestValue entityValue)
    {
        entityValue = requestValue!;
        return requestValue is not null;
    }
}
