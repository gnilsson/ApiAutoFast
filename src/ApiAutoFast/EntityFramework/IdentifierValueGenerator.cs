using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace ApiAutoFast;

public class IdentifierValueGenerator : ValueGenerator<Identifier>
{
    public override bool GeneratesTemporaryValues { get; }

    public override Identifier Next(EntityEntry entry)
    {
        return Identifier.New();
    }
}
