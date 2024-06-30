using LiteFlow.Core;
using Microsoft.Extensions.DependencyInjection;

namespace LiteFlow;

public class FlowBuilder<TIn, TOut>(IServiceProvider serviceProvider) : IFlowBuilder<TIn, TOut>
{
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    private readonly List<Func<OperationFunc<TIn, TOut>, OperationFunc<TIn, TOut>>> _middlewares = [];

    public IFlowBuilder<TIn, TOut> UseMiddleware(Func<OperationContext<TIn>, Func<ValueTask<TOut>>, ValueTask<TOut>> middleware)
    {
        ArgumentNullException.ThrowIfNull(middleware);
        _middlewares.Add(next => context => middleware(context, () => next(context)));
        return this;
    }

    public IFlowBuilder<TIn, TOut> UseMiddleware<T>() where T : IMiddleware<TIn, TOut>
    {
        _middlewares.Add(next => context =>
        {
            var middleware = context.ServiceProvider.GetRequiredService<T>();
            return middleware.InvokeAsync(context, next);
        });
        return this;
    }

    public IBuildableFlow<TIn, TOut> Run(OperationFunc<TIn, TOut> operation)
    {

        ArgumentNullException.ThrowIfNull(operation);
        var builder = new BuildableFlow<TIn, TOut>(_middlewares, c => operation.Invoke(new OperationContext<TIn, TOut>(c.Request, default, c.ServiceProvider,
            c.CancellationToken)), _serviceProvider);
        return builder;
    }

    public IBuildableFlow<TIn, TOut> Run<T>() where T : IFunction<TIn, TOut>
    {
        return Run(c =>
        {
            var handler = c.ServiceProvider.GetRequiredService<T>();
            return handler.HandleAsync(c.Request, c.CancellationToken);
        });
    }

}

public class FlowBuilder<TIn>(IServiceProvider serviceProvider) : IFlowBuilder<TIn>
{
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    private readonly List<Func<OperationAction<TIn>, OperationAction<TIn>>> _middlewares = [];

    public IFlowBuilder<TIn> UseMiddleware(Func<OperationContext<TIn>, Func<ValueTask>, ValueTask> middleware)
    {
        ArgumentNullException.ThrowIfNull(middleware);
        _middlewares.Add(next => context => middleware(context, () => next(context)));
        return this;
    }
    public IFlowBuilder<TIn> UseMiddleware<T>() where T : IMiddleware<TIn>
    {
        _middlewares.Add(next => context =>
        {
            var middleware = context.ServiceProvider.GetRequiredService<T>();
            return middleware.InvokeAsync(context, next);
        });
        return this;
    }

    public IBuildableFlow<TIn> Run(OperationAction<TIn> operation)
    {

        ArgumentNullException.ThrowIfNull(operation);
        var builder = new BuildableFlow<TIn>(_middlewares, operation, _serviceProvider);
        return builder;
    }
    public IBuildableFlow<TIn> Run<T>() where T : IAction<TIn>
    {
        return Run(c =>
        {
            var handler = c.ServiceProvider.GetRequiredService<T>();
            return handler.ExecuteAsync(c.Request, c.CancellationToken);
        });
    }

}