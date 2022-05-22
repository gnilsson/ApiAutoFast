//using MR.AspNetCore.Pagination;
using ApiAutoFast.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MR.EntityFrameworkCore.KeysetPagination;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace ApiAutoFast.EntityFramework;

public sealed class QueryExecutor<TEntity> where TEntity : class, IEntity
{
    private static readonly Dictionary<string, KeysetPaginationDirection> _paginationDirectionParams = new()
    {
        ["first"] = KeysetPaginationDirection.Forward,
        ["last"] = KeysetPaginationDirection.Backward
    };

    private static readonly string[] _paginationReferenceParams = new string[] { "before", "after", };

    private readonly DbSet<TEntity> _dbSet;
    private readonly Dictionary<string, Func<string, Expression<Func<TEntity, bool>>>> _stringMethods;

    public QueryExecutor(DbSet<TEntity> dbSet, Dictionary<string, Func<string, Expression<Func<TEntity, bool>>>> stringMethods)
    {
        _dbSet = dbSet;
        _stringMethods = stringMethods;
    }

    public async IAsyncEnumerable<TEntity> ExecuteAsync(IQueryCollection queryCollection, [EnumeratorCancellation] CancellationToken ct)
    {
        var queries = queryCollection.ToDictionary(x => x.Key, x => x.Value);

        Expression<Func<TEntity, bool>> predicate = p => true;
        KeysetPaginationDirection? direction = null!;
        var referenceId = Identifier.Empty;

        if (queries.Count > 0)
        {
            foreach (var item in queries)
            {
                if (_paginationDirectionParams.TryGetValue(item.Key, out var dir) && direction is null)
                {
                    if (bool.TryParse((string)item.Value, out var value) is false || value is false) continue;

                    direction = dir;
                }
                else if (_paginationReferenceParams.Contains(item.Key) && referenceId == Identifier.Empty)
                {
                    if (Identifier.TryParse(item.Value, out var identifier))
                    {
                        referenceId = identifier;
                    }
                }
                else if (_stringMethods.TryGetValue(item.Key, out var func))
                {
                    predicate = ExpressionUtility.AndAlso(predicate, func(item.Key));
                }
            }
        }

        var query = _dbSet
            .AsNoTracking()
            .Where(predicate)
            .KeysetPaginateQuery(x => x.Descending(x => x.Id),
            direction ?? KeysetPaginationDirection.Forward,
            referenceId != Identifier.Empty ? await _dbSet.FindAsync(new object?[] { referenceId }, cancellationToken: ct) : null)
            .AsAsyncEnumerable();

        await foreach (var entity in query)
        {
            yield return entity;
        }
    }
}
