using ApiAutoFast.Sample.Server.Database.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.RegularExpressions;

namespace ApiAutoFast.Sample.Server.Database;

public sealed class PublicationDateTime : ComplexOf<DateTime, PublicationDateTime>
{
    protected override bool TryConvertValidate(string item, out DateTime entityValue)
    {
        if (DateTime.TryParse(item, out entityValue))
        {
            return true;
        }

        return false;
    }

    public override string ToString() => EntityValue.ToLongDateString();
    protected override string MessageOnFailedValidation => "Could not convert string to DateTime.";
}


public class DateValueConverter : ValueConverter<PublicationDateTime, DateTime>
{
    public DateValueConverter() : base(
        s => s.EntityValue,
        t => PublicationDateTime.ConvertFrom(t))
    { }
}

public class Title : DefaultString
{
    const string RegexPattern = "";

    protected override bool TryConvertValidate(string requestValue, out string entityValue)
    {
        if (Regex.IsMatch(requestValue, RegexPattern))
        {
            entityValue = requestValue;
            return true;
        }

        entityValue = string.Empty;
        return false;
    }
}

public class PostType : ComplexOf<EPostType, PostType>
{
    public override string ToString() => EntityValue.ToString();
}

