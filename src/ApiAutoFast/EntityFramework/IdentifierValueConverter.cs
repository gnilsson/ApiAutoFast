using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ApiAutoFast;

public class IdentifierValueConverter : ValueConverter<Identifier, Guid>
{
    public IdentifierValueConverter() : base(
        s => s,
        t => new Identifier(t))
    { }
}

public class SequentialIdentifierValueConverter : ValueConverter<SequentialIdentifier, Guid>
{
    public SequentialIdentifierValueConverter() : base(
        s => s,
        t => new SequentialIdentifier(t))
    { }
}

//public class SequentialIdentifierValueConverter : ValueConverter<SequentialIdentifier?, Guid>
//{
//    public SequentialIdentifierValueConverter() : base(
//        s => (Guid)s!,
//        t => new SequentialIdentifier(t))
//    { }
//}
