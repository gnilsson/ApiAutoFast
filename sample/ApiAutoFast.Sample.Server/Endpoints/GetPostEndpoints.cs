//using ApiAutoFast.EntityFramework;
//using ApiAutoFast.Utility;
//using FastEndpoints;
//using FluentValidation.Results;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Primitives;
//using MR.AspNetCore.Pagination;
//using MR.EntityFrameworkCore.KeysetPagination;
//using System.Diagnostics.CodeAnalysis;
//using System.Linq.Expressions;

//namespace ApiAutoFast.Sample.Server;

//public class GetPost2Endpoint : Endpoint<PostQueryRequest, Paginated<PostResponse>, PostMappingProfile>
//{
//    private readonly QueryExecutor<Post> _queryExecutor;
//    private readonly AutoFastSampleDbContext _dbContext;
//    private bool _overrideConfigure = false;

//    public GetPost2Endpoint(AutoFastSampleDbContext dbContext)
//    {
//        _dbContext = dbContext;
//        _queryExecutor = new QueryExecutor<Post>(_dbContext.Posts, _stringMethods);
//    }

//    public override void Configure()
//    {
//        if (_overrideConfigure is false)
//        {
//            Verbs(Http.GET);
//            Routes("/posts2");
//            // note: temporarily allow anonymous
//            AllowAnonymous();
//        }
//    }

//    private static readonly Dictionary<string, KeysetPaginationDirection> _paginationDirectionParams = new()
//    {
//        ["first"] = KeysetPaginationDirection.Forward,
//        ["last"] = KeysetPaginationDirection.Backward
//    };

//    private static readonly string[] _paginationReferenceParams = new string[] { "before", "after", };

//    private void AddError(string property, string message)
//    {
//        ValidationFailures.Add(new ValidationFailure(property, message));
//    }

//    private bool HasError()
//    {
//        return ValidationFailures.Count > 0;
//    }

//    public override async Task HandleAsync(PostQueryRequest req, CancellationToken ct)
//    {
//        var f = HttpContext.Request.Query.ToArray();
//        var f2 = HttpContext.Request.Query.ToDictionary(x => x.Key, x => x.Value);

//        Expression<Func<Post, bool>> predicate = p => true;



//        KeysetPaginationDirection? direction = null!;
//        var referenceId = Identifier.Empty;

//        if (f2.Count > 0)
//        {
//            foreach (var item in f2)
//            {
//                if (_paginationDirectionParams.TryGetValue(item.Key, out var dir) && direction is null)
//                {
//                    if (bool.TryParse((string)item.Value, out var value) is false || value is false) continue;

//                    direction = dir;
//                }
//                else if (_paginationReferenceParams.Contains(item.Key) && referenceId == Identifier.Empty)
//                {
//                    if (Identifier.TryParse(item.Value, out var identifier))
//                    {
//                        referenceId = identifier;
//                    }
//                }
//                else if (_stringMethods.TryGetValue(item.Key, out var func))
//                {
//                    predicate = ExpressionUtility.AndAlso(predicate, func(item.Key));
//                }
//            }

//        }

//        //async IAsyncEnumerable<PostResponse> YieldResponse(IAsyncEnumerable<Post> entities)
//        //{
//        //    await foreach (var entity in entities)
//        //    {
//        //        yield return Map.FromEntity(entity);
//        //    }
//        //}

//        //var f = _dbContext.Posts.Where(x => ((string)x.Title).Contains(req.Title!)).AsAsyncEnumerable();

//        //var r = YieldResponse(f);

//        //var queries = BuildQuery(HttpContext.Request.QueryString.Value);

//        //var usersPaginationResult = await _paginationService.KeysetPaginateAsync(
//        //    _dbContext.Posts,
//        //     (KeysetPaginationBuilder<Post> b) => b.Descending(x => x.CreatedDateTime),
//        //    async id => await _dbContext.Posts.FindAsync(id) ?? null!,
//        //    query => query.Select(x => Map.FromEntity(x)));


//        //var query = _dbContext.Posts
//        //    .AsNoTracking()
//        //    .Where(predicate)
//        //    .KeysetPaginateQuery(x => x.Descending(x => x.Id),
//        //    direction ?? KeysetPaginationDirection.Forward,
//        //    referenceId != Identifier.Empty ? await _dbContext.Posts.FindAsync(new object?[] { referenceId }, cancellationToken: ct) : null)
//        //    .AsAsyncEnumerable();

//        //var query = keysetContext.Query.Take(20).ToArrayAsync();
//        //KeysetPaginationResult<PostResponse> ff = await _paginationService.KeysetPaginateAsync<Post, PostResponse>(
//        //    _dbContext.Posts,
//        //    b => b.Descending(x => x.CreatedDateTime),
//        //    async x => await _dbContext.Posts.Where(queries),
//        //    query => query.Select(x => Map.FromEntity(x)));


//        //var entities =
//        //var responses = YieldResponse(entities);


//        //// var r = f.Select(x => Map.FromEntity(x));

//        //var pag = new Paginated<PostResponse> { Data2 = responses };

//      //  await SendOkAsync(pag);
//        return;
//    }

//    private async IAsyncEnumerable<Post> YieldResponse(Expression<Func<Post, bool>> queries)
//    {
//        await foreach (var ah in _dbContext.Posts.Where(queries).ToAsyncEnumerable())
//        {
//            yield return ah;
//        }
//    }


//    //private async IAsyncEnumerable<PostResponse> YieldRespons2e(Expression<Func<Post, bool>> queries)
//    //{
//    //    await foreach (var ah in _dbContext.Posts.Where(queries).ToAsyncEnumerable())
//    //    {
//    //        yield return Map.FromEntity(ah);
//    //    }
//    //}

//    //private async IAsyncEnumerable<PostResponse> YieldResponse(IAsyncEnumerable<Post>? ahh)
//    //{
//    //    await foreach (var ah in ahh)
//    //    {
//    //        yield return Map.FromEntity(ah);
//    //    }
//    //}

//    //private static readonly Dictionary<string, Func<DbSet<Post>, string, IQueryable<Post>>> _stringMethods = new()
//    //{
//    //    ["Title"] = static (x, z) => x.Where(y => y.Title.EntityValue.Contains(z)),
//    //    ["Description"] = static (x, z) => x.Where(y => y.Description.EntityValue.Contains(z)),
//    //};

//    private static readonly Dictionary<string, Func<string, Expression<Func<Post, bool>>>> _stringMethods = new()
//    {
//        ["Title"] = static x => y => ((string)y.Title).Contains(x),
//        ["Description"] = static x => y => ((string)y.Description).Contains(x),
//    };


//    public static Expression<Func<Post, bool>> BuildQuery(KeyValuePair<string, StringValues>[] queryParameters)
//    {
//        Expression<Func<Post, bool>> predicate = p => true;

//        if (queryParameters.Length <= 0) return predicate;

//        foreach (var parameter in queryParameters)
//        {
//            var stringMethod = _stringMethods[parameter.Key](parameter.Value);

//            predicate = ExpressionUtility.AndAlso(predicate, stringMethod);
//        }

//        return predicate;
//    }

//    //public static Expression<Func<Post, bool>> BuildQuery(string? queryString)
//    //{
//    //    Expression<Func<Post, bool>> predicate = p => true;

//    //    if (string.IsNullOrWhiteSpace(queryString)) return predicate;

//    //    var first = true;
//    //    foreach (var splittedQuery in queryString.Split('&'))
//    //    {
//    //        var query = first ? splittedQuery.Replace("?", "") : splittedQuery;
//    //        first = false;

//    //        var queryPart = query.Split('=');
//    //        var stringMethod = _stringMethods[queryPart[0]](queryPart[1]);
//    //        predicate = ExpressionUtility.AndAlso(predicate, stringMethod);
//    //    }

//    //    return predicate;
//    //}
//}
