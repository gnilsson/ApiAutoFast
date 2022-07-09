using MassTransit;

namespace ApiAutoFast.Domain;

public readonly struct SequentialIdentifier
{
    private readonly Identifier _identifier;
    private readonly long _timestampTicks;

    public SequentialIdentifier(in NewId newIdValue)
    {
        _identifier = new Identifier(newIdValue.ToGuid());
        _timestampTicks = newIdValue.Timestamp.Ticks;
    }

    public static SequentialIdentifier New() => new(NewId.Next());

    public static bool TryParse(string? valueToParse, out SequentialIdentifier seqIdentifier)
    {
        if (Identifier.TryParse(valueToParse, out var identifier)
            && TryToSequentialIdentifier(identifier, out seqIdentifier))
        {
            return true;
        }

        seqIdentifier = default;
        return false;
    }

    public static SequentialIdentifier ConvertFromRequest(string request, Action<string, string> addError)
    {
        if (TryParse(request, out var seqIdentifier)) return seqIdentifier;

        addError(nameof(SequentialIdentifier), "Error while parsing.");

        return default;
    }

    public readonly long TimestampTicks => _timestampTicks;

    private static bool TryToSequentialIdentifier(Identifier identifier, out SequentialIdentifier seqIdentifier)
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

    public static implicit operator Identifier(SequentialIdentifier seqIdentifier) => seqIdentifier._identifier;

    public static bool operator ==(SequentialIdentifier id1, SequentialIdentifier id2) => id1._identifier.GuidValue.Equals(id2._identifier.GuidValue);
    public static bool operator !=(SequentialIdentifier id1, SequentialIdentifier id2) => !id1._identifier.GuidValue.Equals(id2._identifier.GuidValue);
    public static bool operator ==(Guid id1, SequentialIdentifier id2) => id1.Equals(id2._identifier.GuidValue);
    public static bool operator !=(Guid id1, SequentialIdentifier id2) => !id1.Equals(id2._identifier.GuidValue);
}
