using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace ApiAutoFast.Tests;


[UsesVerify]
public class MapsterMapperSnapshotTest
{
    [Fact]
    public Task IsPartiallyGenerated()
    {
        var source = @"
using System;
using ApiAutoFast;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ApiAutoFast.Sample.Server.Database;

public static partial class PostMapper
{
    //public void Adapt()
    //{
    //}
}
";

        return TestHelper.Verify(source);
    }
}