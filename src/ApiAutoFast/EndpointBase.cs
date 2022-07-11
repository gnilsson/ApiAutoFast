using FastEndpoints;
using FluentValidation.Results;

namespace ApiAutoFast;

public abstract class EndpointBase<TRequest, TResponse, TMapping> : Endpoint<TRequest, TResponse, TMapping>
    where TRequest : notnull, new()
    where TResponse : notnull, new()
    where TMapping : notnull, IEntityMapper, new()
{
    public EndpointBase()
    {
        _handleRequestAsync = HandleRequestAsync;
    }

    private bool _saveChanges;
    public void SkipSaveChanges() => _saveChanges = false;
    public bool ShouldSave() => _saveChanges;

    public virtual void ConfigureAutoFast()
    { }

    public bool HasError() => ValidationFailures.Count > 0;

    public void AddError(string property, string message) => ValidationFailures.Add(new ValidationFailure(property, message));

    protected readonly Func<TRequest, CancellationToken, Task> _handleRequestAsync;

    public abstract Task HandleRequestAsync(TRequest req, CancellationToken ct);
}
