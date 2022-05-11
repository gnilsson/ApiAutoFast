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

public enum ProfessionCategory
{
    None = 0,
    Unemployed,
    Programmer,
    CoalmineWorker,
    Botanist,
    SpacestationArchitect,
    Dragon
}

[AutoFastEndpoints]
public class AuthorConfig
{
    internal class Properties
    {
        [Required, CreateCommand, ModifyCommand, QueryRequest]
        public string? FirstName { get; set; }
        [Required, CreateCommand, ModifyCommand, QueryRequest]
        public string? LastName { get; set; }
        [Required, QueryRequest, CreateCommand, ModifyCommand]
        public ProfessionCategory? Profession { get; set; }
        public ICollection<BlogConfig>? Blogs { get; set; }
    }
}

[AutoFastEndpoints]
public class BlogConfig
{
    internal class Properties
    {
        [Required, CreateCommand, ModifyCommand, QueryRequest]
        public string? Title { get; set; }
        [Required]
        public AuthorConfig? Author { get; set; }
        [Required, CreateCommand, ModifyCommand, QueryRequest]
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

    [Fact]
    public Task GeneratesEntitiesCorrectly()
    {
        var source = @"
using ApiAutoFast;
using System.ComponentModel.DataAnnotations;

namespace ApiAutoFast.Sample.Server.Database;

[AutoFastEndpoints]
public class CarCategoryConfig
{
    internal class Properties
    {
        public string Name { get; set; } = default!;
        public PricingModel PricingModel { get; set; } = default!;
        public ICollection<CarModelConfig>? CarModels { get; set; }
    }
}

[AutoFastEndpoints]
public class CarConfig
{
    internal class Properties
    {
        public CarModelConfig CarModel { get; set; } = default!;
        public string LicensePlateNumber { get; set; } = default!;
    }
}

[AutoFastEndpoints]
public class CarModelConfig
{
    internal class Properties
    {
        public string Name { get; set; } = default!;
        public CarCategoryConfig CarCategory { get; set; } = default!;
        public ICollection<CarConfig>? Cars { get; set; }
        public decimal KilometerPrice { get; set; }
    }
}
";
        return TestHelper.Verify(source);
    }

}
