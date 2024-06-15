using LiteFlow.Core;

namespace LiteFlow;

internal class BuildableFlow<TIn, TOut>(
    IEnumerable<Func<OperationDelegate<TIn, TOut>, OperationDelegate<TIn, TOut>>> middlewares,
    OperationDelegate<TIn, TOut> operationDelegate,
    IServiceProvider serviceProvider)
    : IBuildableFlow<TIn, TOut>
{
    private readonly IEnumerable<Func<OperationDelegate<TIn, TOut>, OperationDelegate<TIn, TOut>>> _middlewares = middlewares ?? throw new ArgumentNullException(nameof(middlewares));
    private OperationDelegate<TIn, TOut> _operationDelegate = operationDelegate ?? throw new ArgumentNullException(nameof(operationDelegate));
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    public IFlow<TIn, TOut> Build()
    {
        var middlewares = _middlewares.ToList();
        for (var m = middlewares.Count - 1; m >= 0; m--)
        {
            _operationDelegate = middlewares[m](_operationDelegate);
        }
        return new Flow<TIn, TOut>(_operationDelegate, _serviceProvider);
    }
}