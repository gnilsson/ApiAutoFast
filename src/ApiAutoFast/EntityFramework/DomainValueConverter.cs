using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ApiAutoFast;

// note: this is fun and all but a much smarter decision is probably to use mapster from DomainValue to sql values
public class DomainValueConverter<TRequest, TEntity, TDomain> : ValueConverter<TDomain, TEntity>
    where TDomain : DomainValue<TRequest, TEntity, TDomain>, new()
{
    public DomainValueConverter() : base(
        s => s.EntityValue,
        t => (TDomain)t)
    { }
}
