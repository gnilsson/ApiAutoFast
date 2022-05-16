using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ApiAutoFast;

public class DomainValueConverter<TRequest, TEntity, TResponse, TDomain> : ValueConverter<TDomain, TEntity>
    where TDomain : DomainValue<TRequest, TEntity, TResponse, TDomain>, new()
{
    public DomainValueConverter() : base(s => s.EntityValue, t => (TDomain)t)
    { }
}
