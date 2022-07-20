using System.Diagnostics.CodeAnalysis;

namespace ApiAutoFast;

public class DomainValue<TRequestEntityResponse, TDomain> : DomainValue<TRequestEntityResponse, TRequestEntityResponse, TDomain>
    where TDomain : DomainValue<TRequestEntityResponse, TRequestEntityResponse, TDomain>, new()
{
    protected override bool TryValidateRequestConversion(TRequestEntityResponse? requestValue, [NotNullWhen(true)] out TRequestEntityResponse entityValue)
    {
        entityValue = requestValue!;
        return requestValue is not null;
    }

    public override TRequestEntityResponse ToResponse()
    {
        return EntityValue!;
    }
}
