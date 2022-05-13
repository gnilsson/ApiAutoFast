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
}


[AutoFastContext]
public partial class AutoFastSampleDbContext : DbContext
{
}

";

        return TestHelper.Verify(source);
    }
}