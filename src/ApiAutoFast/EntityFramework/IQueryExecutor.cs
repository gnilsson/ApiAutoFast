using Microsoft.AspNetCore.Http;

namespace ApiAutoFast.EntityFramework;

public interface IQueryExecutor<TEntity> where TEntity : class, IEntity<Identifier>
{
    IAsyncEnumerable<TEntity> ExecuteAsync(IQueryCollection queryCollection, CancellationToken ct);
}
