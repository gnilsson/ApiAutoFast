using Microsoft.AspNetCore.Http;

namespace ApiAutoFast.EntityFramework;

public interface IQueryExecutor<TEntity> where TEntity : class, IEntity<SequentialIdentifier>
{
    IAsyncEnumerable<TEntity> ExecuteAsync(IQueryCollection queryCollection, CancellationToken ct);
}
