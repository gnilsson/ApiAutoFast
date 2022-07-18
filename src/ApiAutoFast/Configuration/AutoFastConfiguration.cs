using ApiAutoFast.Domain;

namespace ApiAutoFast;

public abstract class AutoFastConfiguration
{
    public abstract void Configure();

    public static void AddValidationErrorMessage(string domainValueName, string errorMessage)
    {
        ValidationErrorMessageContainer.Values.Add(domainValueName, errorMessage);
    }

    public static void AddValidationErrorMessage<TDomain>(string errorMessage)
    {
        ValidationErrorMessageContainer.Values.Add(typeof(TDomain).Name, errorMessage);
    }
}
