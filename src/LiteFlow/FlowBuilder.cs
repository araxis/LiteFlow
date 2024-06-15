using LiteFlow.Core;

namespace LiteFlow;

public class FlowBuilder<TIn, TOut> : IFlowBuilder<TIn, TOut>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ServiceProviderFactory _serviceProviderFactory;
    private readonly List<Func<OperationDelegate<TIn, TOut>, OperationDelegate<TIn, TOut>>> _middlewares = [];
    public FlowBuilder(IServiceProvider serviceProvider, ServiceProviderFactory serviceProviderFactory)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _serviceProviderFactory = serviceProviderFactory ?? throw new ArgumentNullException(nameof(serviceProviderFactory));
    }

    public FlowBuilder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _serviceProviderFactory = sp => sp;
    }

    public IFlowBuilder<TIn, TOut> UseMiddleware(Func<OperationContext<TIn>, Func<ValueTask<TOut>>, ValueTask<TOut>> middleware)
    {
        ArgumentNullException.ThrowIfNull(middleware);
        _middlewares.Add(next => context => middleware(context, () => next(context)));
        return this;
    }

    public IBuildableFlow<TIn, TOut> Run(OperationDelegate<TIn, TOut> operation)
    {

        ArgumentNullException.ThrowIfNull(operation);
        var serviceProvider = _serviceProviderFactory.Invoke(_serviceProvider);
        var builder = new BuildableFlow<TIn, TOut>(_middlewares, operation, serviceProvider);
        return builder;
    }

}