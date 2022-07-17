//using Microsoft.EntityFrameworkCore;

//namespace ApiAutoFast.EntityFramework;

//internal interface IRepository<TEntity> where TEntity : class
//{
//    public DbSet<TEntity> Set { get; }
//}

//public class Repository<TEntity, TContext> : IRepository<TEntity> where TEntity : class where TContext : DbContext
//{
//    private readonly TContext _context;
//    private DbSet<TEntity>? _entity;

//    public Repository(TContext context)
//    {
//        _context = context;
//        context.
//    }

//    public DbSet<TEntity> Set => _entity ??= _context.Set<TEntity>();
//}

//public interface IContextWrapper<TContext> where TContext : DbContext
//{
//    TContext Context { get; }
//}

//public class ContextWrapper<TContext> : IContextWrapper<TContext> where TContext : DbContext
//{
//    public ContextWrapper(TContext context)
//    {
//        Context = context;
//    }

//    public TContext Context { get; }
//}
