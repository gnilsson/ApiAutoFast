﻿using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace ApiAutoFast.Tests;

[UsesVerify]
public class SnapshotTests
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
   // public PostsRelation Posts { get; set; } = default!;
    public Title2 Title { get; set; } = default!;
}

public class Title2 : StringDomainValue<Title2>
{

}


//[AutoFastEndpoints]
//public class PostEntity
//{
//    public LikeCount LikeCount { get; set; } = default!;
//    public BlogRelation Blog { get; set; } = default!;
//    public Title Tit { get; set; } = default!;
//    public PublicationDateTime PublicationDateTime { get; set; } = default!;
//    public Description Description { get; set; } = default!;
//    public PostType PostType { get; set; } = default!;
//}



//public class PostsRelation : DomainValue<ICollection<Post>, PostsRelation>
//{

//}

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

[IncludeInCommand(new Type[] { typeof(Blog) })]
public class LikeCount : DomainValue<int, LikeCount>
{ }

//[IncludeInCommand(typeof(Blog), typeof(Post))]
//public class Outsider : DomainValue<string, Outsider>
//{ }

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