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
    private readonly string[] _relationalNavigationNames;

    public QueryExecutor(
        DbSet<TEntity> dbSet,
        Dictionary<string, Func<string, Expression<Func<TEntity, bool>>>> stringMethods,
        string[] relationalNavigationNames)
    {
        _dbSet = dbSet;
        _stringMethods = stringMethods;
        _relationalNavigationNames = relationalNavigationNames;
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
                    && bool.TryParse(param.Value, out var value)
                    && value)
                {
                    direction = dir;
                }
                else if (_paginationReferenceParams.Contains(param.Key)
                    && referenceId == Identifier.Empty
                    && Identifier.TryParse(param.Value, out var identifier))
                {
                    referenceId = identifier;
                }
                else if (_stringMethods.TryGetValue(param.Key, out var func))
                {
                    predicate = ExpressionUtility.AndAlso(predicate, func(param.Value));
                }
            }
        }

        var baseQuery = _dbSet
            .AsNoTracking()
            .Where(predicate);

        foreach (var relationalNavigationName in _relationalNavigationNames)
        {
            baseQuery = baseQuery.Include(relationalNavigationName);
        }

        var query = baseQuery
            .Take(20)
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
