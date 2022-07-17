
using ApiAutoFast;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ApiAutoFast.Sample.Server;

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

    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Blog> Blogs => Set<Blog>();
}
