using ApiAutoFast.SourceGenerator;
using Microsoft.VisualStudio.TestPlatform.Utilities;
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
        public ICollection<BlogCunfig>? Blogs { get; set; }
    }
}

[AutoFastEndpoints(""Blog"")]
public class BlogCunfig
{
    internal class Properties
    {
        [CreateCommand, ModifyCommand, QueryRequest]
        public string Title { get; set; } = default!;
        [Required]
        public AuthorConfig Author { get; set; } = default!;
        [CreateCommand, QueryRequest]
        public Identifier AuthorId { get; set; }
    }
}

";
        //var (diagnostics, output) = TestHelper.GetGeneratedOutput<ApiGenerator>(source);

        //return Verifier
        //    .Verify(output)
        //    .UseDirectory("Snapshots");

        return TestHelper.Verify(source);
    }
}
