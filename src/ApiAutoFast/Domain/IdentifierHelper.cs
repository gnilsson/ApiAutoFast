using MassTransit;

namespace ApiAutoFast.Domain;

public static class IdentifierHelper
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

    //note: wait for statc interface members in c# 11..
    public static bool TryParse<TId>(string? value, out IIdentifier id) where TId : IIdentifier
    {
        if (typeof(TId) == typeof(SequentialIdentifier))
        {
            var parsed = SequentialIdentifier.TryParse(value, out var seqId);
            id = seqId;
            return parsed;
        }

        var p = Identifier.TryParse(value, out var id2);
        id = id2;
        return p;
    }

    private static void FromSequentialByteArray(in byte[] bytes, out int a, out int b, out int c, out int d)
    {
        a = bytes[3] << 24 | bytes[2] << 16 | bytes[1] << 8 | bytes[0];
        b = bytes[5] << 24 | bytes[4] << 16 | bytes[7] << 8 | bytes[6];
        c = bytes[8] << 24 | bytes[9] << 16 | bytes[10] << 8 | bytes[11];
        d = bytes[12] << 24 | bytes[13] << 16 | bytes[15] << 8 | bytes[14];
    }
}
