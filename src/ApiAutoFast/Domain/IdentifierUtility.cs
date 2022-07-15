using MassTransit;

namespace ApiAutoFast;

public static class IdentifierUtility
{
    public static NewId FromSequentialGuid(in Guid guid)
    {
        var bytes = guid.ToByteArray();

        FromSequentialByteArray(bytes, out var a, out var b, out var c, out var d);

        return new NewId(a, b, c, d);
    }

    public static NewId ToNewIdFromSequential(this Guid guid)
    {
        var newId = FromSequentialGuid(guid);

        //inb4 y3k
        if (newId.Timestamp.Year.ToString()[0] != '2')
        {
            throw new InvalidOperationException("Guid must be sequential.");
        }

        return newId;
    }

    //note: wait for static interface members in c# 11..
    public static bool TryParse<TId>(string? value, out TId id) where TId : struct, IIdentifier
    {
        bool parsed;
        if (typeof(TId) == typeof(SequentialIdentifier))
        {
            parsed = SequentialIdentifier.TryParse(value, out var seqIdentifier);
            id = (TId)(IIdentifier)seqIdentifier;
        }
        else
        {
            parsed = Identifier.TryParse(value, out var identifier);
            id = (TId)(IIdentifier)identifier;
        }
        return parsed;
    }

    public static TId ConvertFromRequest<TId>(in string? request, in Action<string, string> addError) where TId : struct, IIdentifier
    {
        if (TryParse<TId>(request, out var identifier)) return identifier;

        addError(nameof(Identifier), "Error while parsing.");

        return default;
    }

    private static void FromSequentialByteArray(in byte[] bytes, out int a, out int b, out int c, out int d)
    {
        a = bytes[3] << 24 | bytes[2] << 16 | bytes[1] << 8 | bytes[0];
        b = bytes[5] << 24 | bytes[4] << 16 | bytes[7] << 8 | bytes[6];
        c = bytes[8] << 24 | bytes[9] << 16 | bytes[10] << 8 | bytes[11];
        d = bytes[12] << 24 | bytes[13] << 16 | bytes[15] << 8 | bytes[14];
    }
}
