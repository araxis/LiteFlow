namespace LiteFlow.Core;

public class OperationContext<T>(T request, IServiceProvider serviceProvider, CancellationToken cancellationToken)
{
    public T Request { get; } = request ?? throw new ArgumentNullException(nameof(request));
    public IServiceProvider ServiceProvider { get; } = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    public CancellationToken CancellationToken { get; } = cancellationToken;
}
public class OperationContext<TRequest,TResult>(TRequest request,TResult result, IServiceProvider serviceProvider, CancellationToken cancellationToken)
{
    public TRequest Request { get; } = request ?? throw new ArgumentNullException(nameof(request));
    public TResult Result { get; }= result ?? throw new ArgumentNullException(nameof(result));
    public IServiceProvider ServiceProvider { get; } = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    public CancellationToken CancellationToken { get; } = cancellationToken;
}