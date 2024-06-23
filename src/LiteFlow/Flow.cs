using LiteFlow.Core;

namespace LiteFlow;

internal class Flow<TIn, TOut>(OperationFunc<TIn, TOut> operationDelegate, IServiceProvider serviceProvider) : IFlow<TIn, TOut>
{
    private readonly OperationFunc<TIn, TOut> _operationDelegate = operationDelegate ?? throw new ArgumentNullException(nameof(operationDelegate));
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    public ValueTask<TOut> ExecuteAsync(TIn input, CancellationToken cancellationToken = default)
    {
        var context = new OperationContext<TIn>(input, _serviceProvider, cancellationToken);
        return _operationDelegate.Invoke(context);
    }
}

internal class Flow<TIn>(OperationAction<TIn> operationDelegate, IServiceProvider serviceProvider) : IFlow<TIn>
{
    private readonly OperationAction<TIn> _operationDelegate = operationDelegate ?? throw new ArgumentNullException(nameof(operationDelegate));
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    public ValueTask ExecuteAsync(TIn input, CancellationToken cancellationToken = default)
    {
        var context = new OperationContext<TIn>(input, _serviceProvider, cancellationToken);
        return _operationDelegate.Invoke(context);
    }
}