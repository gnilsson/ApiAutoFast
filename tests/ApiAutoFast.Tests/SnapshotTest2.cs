using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace ApiAutoFast.Tests;

[UsesVerify]
public class SnapshotTests2
{
    [Fact]
    public Task GeneratesCorrectly()
    {
        var source = @"
using System;
using ApiAutoFast;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ApiAutoFast.Sample.Server.Database;

[AutoFastEndpoints]
public class PostConfig
{
    public Title Title { get; set; } = default!;
    public PublicationDateTime PublicationDateTime { get; set; } = default!;
    public Description Description { get; set; } = default!;
   // public PostType PostType { get; set; } = default!;
}


[AutoFastContext]
public partial class AutoFastSampleDbContext : DbContext
{

}

public class PublicationDateTime : DomainValue<string, DateTime, PublicationDateTime>
{
    protected override bool TryValidateRequestConversion(string requestValue, out DateTime entityValue)
    {
        return DateTime.TryParse(requestValue, out entityValue);
    }

    // note: figure this one out
    public override string ToString() => EntityValue.ToLongDateString();
}


public class Title : DomainValue<string, Title>
{
    private const string RegexPattern = "";

    protected override bool TryValidateRequestConversion(string requestValue, out string entityValue)
    {
        var success = base.TryValidateRequestConversion(requestValue, out _) && Regex.IsMatch(requestValue, RegexPattern);
        entityValue = requestValue;
        return success;
    }

    protected override string? MessageOnFailedValidation => ""Incorrect format on Title."";
}

    public class Description : DomainValue<string, Description>
    {

    }


";

        return TestHelper.Verify(source);
    }
}