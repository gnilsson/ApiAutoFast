﻿using System.Text.RegularExpressions;

namespace ApiAutoFast.Sample.Server;

public class PublicationDateTime : DomainValue<string, DateTime, PublicationDateTime>
{
    protected override bool TryValidateRequestConversion(string? requestValue, out DateTime entityValue)
    {
        entityValue = default!;
        return requestValue is not null && DateTime.TryParse(requestValue, out entityValue);
    }

    //deprecate
    public override string ToString() => EntityValue.ToLongDateString();

    public override string ToResponse()
    {
        return EntityValue.ToLongDateString();
    }
}

public class Title : DomainValue<string, Title>
{
    private const string RegexPattern = "";

    protected override bool TryValidateRequestConversion(string? requestValue, out string entityValue)
    {
        entityValue = requestValue!;
        return requestValue is not null && Regex.IsMatch(requestValue, RegexPattern);
    }

    //    protected override string? MessageOnFailedValidation => "Incorrect format on Title.";
}

public class Description : DomainValue<string, Description>
{ }

public class PostType : DomainValue<EPostType, EPostType, string, PostType>
{
    public override string ToResponse()
    {
        return base.ToString();
    }
}

public class FirstName : DomainValue<string, FirstName>
{

}

public class LastName : DomainValue<string, LastName>
{

}

//[IncludeInCommand(typeof(Blog))] // ?
public class LikeCount : DomainValue<int, LikeCount>
{ }
