using Microsoft.AspNetCore.Http;

namespace ApiAutoFast.EntityFramework;

public interface IQueryExecutor<TEntity, TId> where TEntity : class, IEntity<TId> where TId : IIdentifier
{
    IAsyncEnumerable<TEntity> ExecuteAsync(IQueryCollection queryCollection, CancellationToken ct);
}
