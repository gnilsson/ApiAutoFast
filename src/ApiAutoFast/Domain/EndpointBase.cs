﻿using FastEndpoints;
using FluentValidation.Results;

namespace ApiAutoFast;

public abstract class EndpointBase<TRequest, TResponse, TMapping> : Endpoint<TRequest, TResponse, TMapping>
    where TRequest : notnull, new()
    where TResponse : notnull, new()
    where TMapping : notnull, IEntityMapper, new()
{
    private static readonly PairCollection<Http, HttpVerb> _pairs = new()
    {
        new Pair<Http, HttpVerb>(Http.GET, HttpVerb.Get),
        new Pair<Http, HttpVerb>(Http.POST, HttpVerb.Post),
        new Pair<Http, HttpVerb>(Http.PUT, HttpVerb.Put),
        new Pair<Http, HttpVerb>(Http.DELETE, HttpVerb.Delete),
    };

    public EndpointBase()
    {
        _handleRequestAsync = HandleRequestAsync;
    }

    private bool _skipSaveChanges;
    public void SkipSaveChanges() => _skipSaveChanges = true;
    public bool ShouldSave() => !_skipSaveChanges;
    public void MapRoute(string route, HttpVerb verb)
    {
        Routes(route);
        Verbs(_pairs[verb]);
    }
    public bool HasError() => ValidationFailures.Count > 0;
    public void AddError(string property, string message) => ValidationFailures.Add(new ValidationFailure(property, message));
    public abstract Task HandleRequestAsync(TRequest req, CancellationToken ct);

    protected readonly Func<TRequest, CancellationToken, Task> _handleRequestAsync;
}
