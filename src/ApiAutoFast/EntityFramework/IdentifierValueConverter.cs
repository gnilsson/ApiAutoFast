using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ApiAutoFast;

public class IdentifierValueConverter : ValueConverter<Identifier, Guid>
{
    public IdentifierValueConverter() : base(
        s => s,
        t => new Identifier(t))
    { }
}
