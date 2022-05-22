using ApiAutoFast.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
        var queryParameters = queryCollection.ToDictionary(x => x.Key, x => x.Value);

        Expression<Func<TEntity, bool>> predicate = p => true;
        KeysetPaginationDirection? direction = null!;
        var referenceId = Identifier.Empty;

        if (queryParameters.Count > 0)
        {
            foreach (var param in queryParameters)
            {
                if (_paginationDirectionParams.TryGetValue(param.Key, out var dir) && direction is null)
                {
                    if (bool.TryParse((string)param.Value, out var value) is false || value is false) continue;

                    direction = dir;
                }
                else if (_paginationReferenceParams.Contains(param.Key) && referenceId == Identifier.Empty)
                {
                    if (Identifier.TryParse(param.Value, out var identifier) is false) continue;

                    referenceId = identifier;
                }
                else if (_stringMethods.TryGetValue(param.Key, out var func))
                {
                    predicate = ExpressionUtility.AndAlso(predicate, func(param.Value));
                }
            }
        }

        var query = _dbSet
            .AsNoTracking()
            .Where(predicate)
            .Take(2)
            .KeysetPaginateQuery(
            builder => builder.Descending(x => x.Id),
            direction ?? KeysetPaginationDirection.Forward,
            referenceId == Identifier.Empty ? null : await _dbSet.FindAsync(new object?[] { referenceId }, cancellationToken: ct))
            .AsAsyncEnumerable();

        await foreach (var entity in query)
        {
            yield return entity;
        }
    }
}
