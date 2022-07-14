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
    public bool Equals(Identifier other) => _identifier.Equals(other);
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
    public Guid ToGuid() => _identifier.ToGuid();

    public static implicit operator Identifier(in SequentialIdentifier seqIdentifier) => seqIdentifier._identifier;
    public static implicit operator Guid(in SequentialIdentifier seqIdentifier) => seqIdentifier._identifier.ToGuid();
    public static implicit operator string(in SequentialIdentifier seqIdentifier) => seqIdentifier._identifier.ToString();

    public static bool operator <(in SequentialIdentifier id1, in SequentialIdentifier id2) => id1._newId.CompareTo(id2) < 0;
    public static bool operator >(in SequentialIdentifier id1, in SequentialIdentifier id2) => id1._newId.CompareTo(id2) > 0;
    public static bool operator ==(in SequentialIdentifier id1, in SequentialIdentifier id2) => id1._identifier.Equals(id2._identifier);
    public static bool operator !=(in SequentialIdentifier id1, in SequentialIdentifier id2) => !id1._identifier.Equals(id2._identifier);
    public static bool operator ==(in Identifier id1, in SequentialIdentifier id2) => id1.Equals(id2._identifier);
    public static bool operator !=(in Identifier id1, in SequentialIdentifier id2) => !id1.Equals(id2._identifier);
    public static bool operator ==(in SequentialIdentifier id1, in Identifier id2) => id1._identifier.Equals(id2);
    public static bool operator !=(in SequentialIdentifier id1, in Identifier id2) => !id1.Equals(id2);
    public static bool operator ==(in Guid id1, in SequentialIdentifier id2) => id1.Equals(id2._identifier);
    public static bool operator !=(in Guid id1, in SequentialIdentifier id2) => !id1.Equals(id2._identifier);
}

public static class Helper
{

    static void FromSequentialByteArray(in byte[] bytes, out Int32 a, out Int32 b, out Int32 c, out Int32 d)
    {
        a = bytes[3] << 24 | bytes[2] << 16 | bytes[1] << 8 | bytes[0];
        b = bytes[5] << 24 | bytes[4] << 16 | bytes[7] << 8 | bytes[6];
        c = bytes[8] << 24 | bytes[9] << 16 | bytes[10] << 8 | bytes[11];
        d = bytes[12] << 24 | bytes[13] << 16 | bytes[15] << 8 | bytes[14];
    }

    public static NewId FromSequentialGuid(in Guid guid)
    {
        var bytes = guid.ToByteArray();
        FromSequentialByteArray(bytes, out int a, out int b, out int c, out int d);

        return new NewId(a, b, c, d);
    }

    public static NewId ToNewIdFromSequential(this Guid guid)
    {
        var newId = Helper.FromSequentialGuid(guid);

        //inb4 y3k
        if (newId.Timestamp.Year.ToString()[0] != '2')
        {
            throw new InvalidOperationException("Guid must be sequential.");
        }

        return newId;
    }
}
