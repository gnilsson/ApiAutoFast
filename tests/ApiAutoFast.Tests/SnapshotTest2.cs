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
public class BlogEntity
{
    public PostsRelation Posts { get; set; } = default!;
    public Title Title { get; set; } = default!;
}

[AutoFastEndpoints]
public class PostEntity
{
    public BlogRelation Blog { get; set; } = default!;
    public Title Title { get; set; } = default!;
    public PublicationDateTime PublicationDateTime { get; set; } = default!;
    public Description Description { get; set; } = default!;
    public PostType PostType { get; set; } = default!;
    public LikeCount LikeCount { get; set; } = default!;
}



public class PostsRelation : DomainValue<ICollection<Post>, PostsRelation>
{

}

public class BlogRelation : DomainValue<string, Blog, BlogRelation>
{ }

public class PublicationDateTime : DomainValue<string, DateTime, PublicationDateTime>
{
    protected override bool TryValidateRequestConversion(string? requestValue, out DateTime entityValue)
    {
        entityValue = default!;
        return requestValue is not null && DateTime.TryParse(requestValue, out entityValue);
    }

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
}

public class Description : DomainValue<string, Description>
{ }

public class PostType : DomainValue<EPostType, EPostType, string, PostType>
{ }

public class LikeCount : DomainValue<int, LikeCount>
{ }


    [AutoFastContext]
public partial class AutoFastSampleDbContext : DbContext
{

}

public enum EPostType
{
    Text = 0,
    Lyric,
    Haiku,
}
";

        return TestHelper.Verify(source);
    }
}