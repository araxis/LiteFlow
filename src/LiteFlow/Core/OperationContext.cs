namespace LiteFlow.Core;


public class OperationContext<TRequest>(TRequest request, IServiceProvider serviceProvider, CancellationToken cancellationToken)
{
    public IServiceProvider ServiceProvider { get; } = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    public TRequest Request { get; } = request;
    public CancellationToken CancellationToken { get; } = cancellationToken;

    public OperationContext<TRequest, TResponse> Map<TResponse>(TResponse response) =>
        new(Request, response, serviceProvider, CancellationToken);

    public OperationContext<TRequest, TResponse> Map<TResponse>(Func<TResponse> response) => Map(response.Invoke());
}


public class OperationContext<TRequest, TResponse>(TRequest request, TResponse response, IServiceProvider serviceProvider, CancellationToken cancellationToken) : OperationContext<TRequest>(request, serviceProvider,cancellationToken)
{
    public TResponse Response { get; } = response;

}


