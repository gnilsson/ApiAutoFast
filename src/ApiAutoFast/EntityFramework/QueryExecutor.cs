using ApiAutoFast.Descriptive;
using ApiAutoFast.Domain;
using ApiAutoFast.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MR.EntityFrameworkCore.KeysetPagination;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace ApiAutoFast.EntityFramework;

public sealed class QueryExecutor<TEntity, TId> : IQueryExecutor<TEntity, TId> where TEntity : class, IEntity<TId> where TId : struct, IIdentifier
{
    private static readonly Dictionary<string, KeysetPaginationDirection> _paginationDirectionParams = new()
    {
        [QueryParameterText.First] = KeysetPaginationDirection.Forward,
        [QueryParameterText.Last] = KeysetPaginationDirection.Backward
    };

    private static readonly string[] _paginationReferenceParams = new[]
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

        Expression<Func<TEntity, bool>>? predicate = null;
        KeysetPaginationDirection? direction = null;
        IIdentifier? referenceId = null;

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
                && referenceId is null
                && IdentifierHelper.TryParse<TId>(param.Value, out var identifier))
            {
                referenceId = identifier;
            }
            else if (_stringMethods.TryGetValue(param.Key, out var func))
            {
                predicate = predicate is null
                    ? func(param.Value)
                    : ExpressionUtility.AndAlso(predicate, func(param.Value));
            }
        }

        var keysetContext = _dbSet.KeysetPaginate(
            x => x.Descending(y => y.CreatedDateTime),
            direction ?? KeysetPaginationDirection.Forward,
            referenceId is null ? null : await _dbSet.FindAsync(new object?[] { referenceId }, cancellationToken: ct));

        var query = keysetContext.Query
            .AsNoTracking()
            .Take(5);

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        foreach (var relationalNavigationName in _relationalNavigationNames)
        {
            query = query.Include(relationalNavigationName);
        }

        if (keysetContext.Direction is KeysetPaginationDirection.Backward)
        {
            var entities = await query.ToListAsync(ct);

            entities.Reverse();

            foreach (var entity in entities.AsEnumerable())
            {
                yield return entity;
            }
            yield break;
        }

        await foreach (var entity in query.AsAsyncEnumerable().WithCancellation(ct))
        {
            yield return entity;
        }
    }
}
