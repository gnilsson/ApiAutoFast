using MassTransit;
using System.Runtime.CompilerServices;

namespace ApiAutoFast;

public readonly struct SequentialIdentifier :
        IEquatable<SequentialIdentifier>,
        IComparable<SequentialIdentifier>,
        IComparable,
        IFormattable
{
    public static readonly SequentialIdentifier Empty = new(NewId.Empty);

    private readonly Identifier _identifier;
    private readonly NewId _newId;

    public SequentialIdentifier(in NewId newIdValue)
    {
        _identifier = new Identifier(newIdValue.ToSequentialGuid());
        _newId = newIdValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SequentialIdentifier New() => new(NewId.Next());

    public readonly DateTime Timestamp => _newId.Timestamp;

    public static bool TryParse(in string? valueToParse, out SequentialIdentifier seqIdentifier)
    {
        if (Identifier.TryParse(valueToParse, out var identifier)
            && TryToSequentialIdentifier(identifier, out seqIdentifier))
        {
            return true;
        }

        seqIdentifier = default;
        return false;
    }

    public static SequentialIdentifier ConvertFromRequest(in string? request, in Action<string, string> addError)
    {
        if (TryParse(request, out var seqIdentifier)) return seqIdentifier;

        addError(nameof(SequentialIdentifier), "Error while parsing.");

        return default;
    }

    private static bool TryToSequentialIdentifier(in Identifier identifier, out SequentialIdentifier seqIdentifier)
    {
        try
        {
            var newId = identifier.ToGuid().ToNewId();
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
    public string ToString(string? format, IFormatProvider? formatProvider) => _identifier.ToString(null, formatProvider);
    public int CompareTo(SequentialIdentifier other) => _newId.CompareTo(other._newId);
    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;

        if (obj is not SequentialIdentifier) throw new ArgumentException("Argument must be a SequentialIdentifier");

        return CompareTo((SequentialIdentifier)obj);
    }
    public override bool Equals(object? obj) => base.Equals(obj);
    public override int GetHashCode() => base.GetHashCode();
    public override string ToString() => _identifier.ToString();

    public static implicit operator Identifier(in SequentialIdentifier seqIdentifier) => seqIdentifier._identifier;
    public static bool operator <(in SequentialIdentifier id1, in SequentialIdentifier id2) => id1._newId.CompareTo(id2) < 0;
    public static bool operator >(in SequentialIdentifier id1, in SequentialIdentifier id2) => id1._newId.CompareTo(id2) > 0;
    public static bool operator ==(in SequentialIdentifier id1, in SequentialIdentifier id2) => id1._identifier.Equals(id2._identifier);
    public static bool operator !=(in SequentialIdentifier id1, in SequentialIdentifier id2) => !id1._identifier.Equals(id2._identifier);
    public static bool operator ==(in Guid id1, in SequentialIdentifier id2) => id1.Equals(id2._identifier);
    public static bool operator !=(in Guid id1, in SequentialIdentifier id2) => !id1.Equals(id2._identifier);
}
