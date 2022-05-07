using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace ApiAutoFast.Tests;

[UsesVerify]
public class SnapshotTests
{
    [Fact]
    public Task GeneratesContextCorrectly()
    {
        var source = @"
using ApiAutoFast;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ApiAutoFast.Sample.Server.Database;


[AutoFastContext]
public partial class AutoFastSampleDbContext : DbContext
{
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}

[AutoFastEndpoints]
public class AuthorConfig
{
    internal class Properties
    {
        [Required, CreateCommand]
        public string? FirstName { get; set; }
        [CreateCommand, QueryRequest]
        public string? LastName { get; set; }
        [QueryRequest, CreateCommand]
        public ProfessionCategory Profession { get; set; }
    }


    public partial class Command { };

    public partial class Request { };

    public partial class MappingProfile
    {
        // override ToEntity
    }
}
";
        return TestHelper.Verify(source);
    }
}
