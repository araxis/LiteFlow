namespace LiteFlow.Core;

public class Context<TIn>(TIn request)
{
    public TIn Request { get; } = request;
}
public class Context<TIn, TOut>(TIn request, TOut result)
{
    public TIn Request { get; } = request;
    public TOut Response { get; } = result;


}
public class OperationContext<TRequest>(TRequest request, IServiceProvider serviceProvider, CancellationToken cancellationToken) : Context<TRequest>(request)
{
    public IServiceProvider ServiceProvider { get; } = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    public CancellationToken CancellationToken { get; } = cancellationToken;

    public OperationContext<TRequest, TResponse> Map<TResponse>(TResponse response) =>
        new(Request, response, serviceProvider, CancellationToken);

    public OperationContext<TRequest, TResponse> Map<TResponse>(Func<TResponse> response) => Map(response.Invoke());
}


public class OperationContext<TRequest, TResponse>(TRequest request, TResponse response, IServiceProvider serviceProvider, CancellationToken cancellationToken) : OperationContext<TRequest>(request, serviceProvider,cancellationToken)
{
    public TResponse Response { get; } = response;

}


