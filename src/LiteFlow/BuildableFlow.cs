using LiteFlow.Core;

namespace LiteFlow;

internal class BuildableFlow<TIn, TOut> : IBuildableFlow<TIn, TOut>
{
    private readonly IEnumerable<Func<OperationFunc<TIn, TOut>, OperationFunc<TIn, TOut>>> _middlewares;
    private OperationFunc<TIn, TOut> _operationDelegate;
    private readonly IServiceProvider _serviceProvider;

    internal BuildableFlow(IEnumerable<Func<OperationFunc<TIn, TOut>, OperationFunc<TIn, TOut>>> middlewares,
        OperationFunc<TIn, TOut> operationDelegate,
        IServiceProvider serviceProvider)
    {
        _middlewares = middlewares ?? throw new ArgumentNullException(nameof(middlewares));
        _operationDelegate = operationDelegate ?? throw new ArgumentNullException(nameof(operationDelegate));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

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

internal class BuildableFlow<TIn> : IBuildableFlow<TIn>
{
    private readonly IEnumerable<Func<OperationAction<TIn>, OperationAction<TIn>>> _middlewares;
    private OperationAction<TIn> _operationDelegate;
    private readonly IServiceProvider _serviceProvider;

    internal BuildableFlow(IEnumerable<Func<OperationAction<TIn>, OperationAction<TIn>>> middlewares,
        OperationAction<TIn> operationDelegate,
        IServiceProvider serviceProvider)
    {
        _middlewares = middlewares ?? throw new ArgumentNullException(nameof(middlewares));
        _operationDelegate = operationDelegate ?? throw new ArgumentNullException(nameof(operationDelegate));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public IFlow<TIn> Build()
    {
        var middlewares = _middlewares.ToList();
        for (var m = middlewares.Count - 1; m >= 0; m--)
        {
            _operationDelegate = middlewares[m](_operationDelegate);
        }
        return new Flow<TIn>(_operationDelegate, _serviceProvider);
    }
}