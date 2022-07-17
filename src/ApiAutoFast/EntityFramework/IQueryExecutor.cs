using Microsoft.AspNetCore.Http;

namespace ApiAutoFast.EntityFramework;

public interface IQueryExecutor<TEntity, TId>  where TEntity : class, ITimestamp //, IEntity<TId> where TId : struct, IIdentifier
{
    IAsyncEnumerable<TEntity> ExecuteAsync(IQueryCollection queryCollection, CancellationToken ct);
}
