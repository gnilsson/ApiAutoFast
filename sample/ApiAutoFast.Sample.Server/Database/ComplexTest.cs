﻿using ApiAutoFast.Sample.Server.Database.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.RegularExpressions;

namespace ApiAutoFast.Sample.Server.Database;


public class PublicationDateTime : DomainValue<string, DateTime, PublicationDateTime>
{
    protected override bool TryValidateRequestConversion(string? requestValue, out DateTime entityValue)
    {
        entityValue = default!;
        return requestValue is not null && DateTime.TryParse(requestValue, out entityValue);
    }

    // note: figure this one out
    public override string ToString() => EntityValue.ToLongDateString();
}


public class Title : DomainValue<string, Title>
{
    private const string RegexPattern = "";

    protected override bool TryValidateRequestConversion(string? requestValue, out string entityValue)
    {
        entityValue = requestValue!;
        return requestValue is not null && Regex.IsMatch(requestValue, RegexPattern);
    }

    protected override string? MessageOnFailedValidation => "Incorrect format on Title.";
}

public class Description : DomainValue<string, Description>
{

}

public class PostType : DomainValue<EPostType, PostType>
{

}

//public class PublicationDateTimeValueConverter : ValueConverter<PublicationDateTime, DateTime>
//{
//    public PublicationDateTimeValueConverter() : base(
//        s => s.EntityValue,
//        t => PublicationDateTime.ConvertFromEntity(t))
//    { }
//}


//public sealed class PublicationDateTime : ComplexOf<DateTime, PublicationDateTime>
//{
//    protected override bool TryConvertValidate(string item, out DateTime entityValue)
//    {
//        if (DateTime.TryParse(item, out entityValue))
//        {
//            return true;
//        }

//        return false;
//    }

//    public override string ToString() => EntityValue.ToLongDateString();
//    protected override string MessageOnFailedValidation => "Could not convert string to DateTime.";
//}



//public class Title2 : DefaultString
//{
//    const string RegexPattern = "";

//    protected override bool TryConvertValidate(string requestValue, out string entityValue)
//    {
//        if (Regex.IsMatch(requestValue, RegexPattern))
//        {
//            entityValue = requestValue;
//            return true;
//        }

//        entityValue = string.Empty;
//        return false;
//    }
//}

//public class PostType : ComplexOf<EPostType, PostType>
//{
//    public override string ToString() => EntityValue.ToString();
//}

