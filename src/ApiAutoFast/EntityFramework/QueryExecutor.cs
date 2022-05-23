using ApiAutoFast.Descriptive;
using ApiAutoFast.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MR.EntityFrameworkCore.KeysetPagination;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace ApiAutoFast.EntityFramework;

public sealed class QueryExecutor<TEntity> where TEntity : class, IEntity
{
    private static readonly Dictionary<string, KeysetPaginationDirection> _paginationDirectionParams = new()
    {
        [QueryParameterText.First] = KeysetPaginationDirection.Forward,
        [QueryParameterText.Last] = KeysetPaginationDirection.Backward
    };

    private static readonly ImmutableArray<string> _paginationReferenceParams = new()
    {
        QueryParameterText.Before,
        QueryParameterText.After,
    };

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
                if (_paginationDirectionParams.TryGetValue(param.Key, out var dir)
                    && direction is null
                    && (bool.TryParse((string)param.Value, out var value) is false || value is false))
                {
                    direction = dir;
                }
                else if (_paginationReferenceParams.Contains(param.Key)
                    && referenceId == Identifier.Empty
                    && (Identifier.TryParse(param.Value, out var identifier) is false))
                {
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

        await foreach (var entity in query.WithCancellation(ct))
        {
            yield return entity;
        }
    }
}
