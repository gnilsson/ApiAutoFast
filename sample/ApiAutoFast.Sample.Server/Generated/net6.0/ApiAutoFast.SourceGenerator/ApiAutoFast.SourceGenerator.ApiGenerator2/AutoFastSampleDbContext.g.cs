
using ApiAutoFast;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ApiAutoFast.Sample.Server.Database;

public partial class AutoFastSampleDbContext : DbContext
{
    private static readonly Type[] _entityTypes;

    static AutoFastSampleDbContext()
    {
        _entityTypes = AutoFastDbContextHelper.GetEntityTypes<AutoFastSampleDbContext>();
    }

    public AutoFastSampleDbContext(DbContextOptions<AutoFastSampleDbContext> options) : base(options) { }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AutoFastDbContextHelper.UpdateModifiedDateTime(ChangeTracker.Entries());

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        AutoFastDbContextHelper.BuildEntities(modelBuilder, _entityTypes);
    }

    public DbSet<Author> Authors { get; init; } = default!;
}
