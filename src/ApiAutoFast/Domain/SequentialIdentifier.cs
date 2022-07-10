using MassTransit;

namespace ApiAutoFast;

public readonly struct SequentialIdentifier
{
    private readonly Identifier _identifier;
    private readonly long _timestampTicks;

    public readonly long TimestampTicks => _timestampTicks;

    public Identifier Identifier => _identifier;

    public SequentialIdentifier(in NewId newIdValue)
    {
        _identifier = new Identifier(newIdValue.ToGuid());
        _timestampTicks = newIdValue.Timestamp.Ticks;
    }

    public static SequentialIdentifier New() => new(NewId.Next());

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

    public static SequentialIdentifier ConvertFromRequest(in string request, in Action<string, string> addError)
    {
        if (TryParse(request, out var seqIdentifier)) return seqIdentifier;

        addError(nameof(SequentialIdentifier), "Error while parsing.");

        return default;
    }

    private static bool TryToSequentialIdentifier(in Identifier identifier, out SequentialIdentifier seqIdentifier)
    {
        try
        {
            var newId = identifier.GuidValue.ToNewId();
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

    public override bool Equals(object? obj) => base.Equals(obj);
    public override int GetHashCode() => base.GetHashCode();
    public override string ToString() => _identifier.StringValue;

    public static implicit operator Identifier(in SequentialIdentifier seqIdentifier) => seqIdentifier._identifier;
    public static bool operator <(in SequentialIdentifier id1, in SequentialIdentifier id2) => id1._timestampTicks < id2._timestampTicks;
    public static bool operator >(in SequentialIdentifier id1, in SequentialIdentifier id2) => id1._timestampTicks > id2._timestampTicks;
    public static bool operator ==(in SequentialIdentifier id1, in SequentialIdentifier id2) => id1._identifier.GuidValue.Equals(id2._identifier.GuidValue);
    public static bool operator !=(in SequentialIdentifier id1, in SequentialIdentifier id2) => !id1._identifier.GuidValue.Equals(id2._identifier.GuidValue);
    public static bool operator ==(in Guid id1, in SequentialIdentifier id2) => id1.Equals(id2._identifier.GuidValue);
    public static bool operator !=(in Guid id1, in SequentialIdentifier id2) => !id1.Equals(id2._identifier.GuidValue);
}
