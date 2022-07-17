namespace ApiAutoFast.Domain;

internal static class ValidationErrorMessageContainer
{
    public static Dictionary<string, string> Values { get; } = new();

    public static string Get<TDomain>()
    {
        if (Values.TryGetValue(typeof(TDomain).Name, out var errorMessage))
        {
            return errorMessage;
        }

        return "Error when converting request.";
    }
}
