using MassTransit;
using System.Runtime.CompilerServices;

namespace ApiAutoFast;

public readonly struct SequentialIdentifier :
        IIdentifier,
        IComparable<SequentialIdentifier>,
        IComparable,
        IFormattable
{
    public static readonly SequentialIdentifier Empty = new(NewId.Empty);

    private readonly Identifier _identifier;
    public readonly NewId _newId;

    public SequentialIdentifier(in NewId newIdValue)
    {
        _identifier = new Identifier(newIdValue.ToSequentialGuid());
        _newId = newIdValue;
    }

    public SequentialIdentifier(in Guid guidValue)
    {
        _identifier = new Identifier(guidValue);
        _newId = guidValue.ToNewIdFromSequential();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SequentialIdentifier New() => new(NewId.Next());

    public readonly DateTime Timestamp => _newId.Timestamp;

    public static bool TryParse(in string? valueToParse, out SequentialIdentifier seqIdentifier)
    {
        if (Identifier.TryParse(valueToParse, out var identifier)
            && TryToConstruct(identifier, out seqIdentifier))
        {
            return true;
        }

        seqIdentifier = default;
        return false;
    }

    private static bool TryToConstruct(in Guid identifier, out SequentialIdentifier seqIdentifier)
    {
        try
        {
            var newId = identifier.ToNewIdFromSequential();
            _ = newId.Timestamp;
            seqIdentifier = new SequentialIdentifier(newId);
        }
        catch
        {
            seqIdentifier = default;
            return false;
        }

        return true;
    }

    public bool Equals(SequentialIdentifier other) => _identifier.Equals(other._identifier);
    public bool Equals(SequentialIdentifier? other) => _identifier.Equals(other?._identifier);
    public bool Equals(Identifier other) => _identifier.Equals(other);
    public bool Equals(Guid other) => _identifier.Equals(other);
    public string ToString(string? format, IFormatProvider? formatProvider) => _identifier.ToString(null, formatProvider);
    public int CompareTo(SequentialIdentifier other) => _newId.CompareTo(other._newId);
    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;

        if (obj is not SequentialIdentifier) throw new ArgumentException("Argument must be a SequentialIdentifier");

        return CompareTo((SequentialIdentifier)obj);
    }
    public override bool Equals(object? obj) => Equals(obj);
    public override int GetHashCode() => base.GetHashCode();
    public override string ToString() => _identifier;

    public static implicit operator Identifier(SequentialIdentifier seqIdentifier) => seqIdentifier._identifier;
    public static implicit operator Guid(SequentialIdentifier seqIdentifier) => seqIdentifier._identifier;
    public static implicit operator string(SequentialIdentifier seqIdentifier) => seqIdentifier._identifier;

    public static bool operator <(SequentialIdentifier id1, SequentialIdentifier id2) => id1.CompareTo(id2) < 0;
    public static bool operator >(SequentialIdentifier id1, SequentialIdentifier id2) => id1.CompareTo(id2) > 0;

    public static bool operator ==(SequentialIdentifier id1, SequentialIdentifier id2) => id1.Equals(id2);
    public static bool operator !=(SequentialIdentifier id1, SequentialIdentifier id2) => !id1.Equals(id2);
    public static bool operator ==(Identifier id1, SequentialIdentifier id2) => id1.Equals(id2._identifier);
    public static bool operator !=(Identifier id1, SequentialIdentifier id2) => !id1.Equals(id2._identifier);
    public static bool operator ==(SequentialIdentifier id1, Identifier id2) => id1.Equals(id2);
    public static bool operator !=(SequentialIdentifier id1, Identifier id2) => !id1.Equals(id2);
    public static bool operator ==(Guid id1, SequentialIdentifier id2) => id1.Equals(id2._identifier);
    public static bool operator !=(Guid id1, SequentialIdentifier id2) => !id1.Equals(id2._identifier);
}
