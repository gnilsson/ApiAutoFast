
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

    partial void ExtendOnModelCreating(ModelBuilder modelBuilder);
    partial void ExtendSaveChanges();

    public AutoFastSampleDbContext(DbContextOptions<AutoFastSampleDbContext> options) : base(options) { }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AutoFastDbContextHelper.UpdateModifiedDateTime(ChangeTracker.Entries());

        ExtendSaveChanges();

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        AutoFastDbContextHelper.BuildEntities(modelBuilder, _entityTypes);

        ExtendOnModelCreating(modelBuilder);
    }

    public DbSet<Author> Authors { get; init; } = default!;
    public DbSet<Blog> Blogs { get; init; } = default!;
}
