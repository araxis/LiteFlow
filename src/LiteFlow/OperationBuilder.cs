using LiteFlow.Core;
using Microsoft.Extensions.DependencyInjection;

namespace LiteFlow;

public class OperationBuilder<TIn, TOut> : IOperationBuilder<TIn, TOut>
{
    private readonly List<Func<OperationContext<TIn>, Action<TOut>, ValueTask>> _operations;

    internal OperationBuilder() : this([])
    {
    }
    internal OperationBuilder(IEnumerable<Func<OperationContext<TIn>, Action<TOut>, ValueTask>> operations)
    {
        ArgumentNullException.ThrowIfNull(operations);
        _operations = operations.ToList();
    }

    internal OperationBuilder<TIn, TOut> AddOperation(Func<OperationContext<TIn>, Action<TOut>, ValueTask> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        _operations.Add(operation);
        return this;
    }

    public IBuildableOperation<TIn, TOut> AddFunction(OperationFunc<TIn, TOut> operation)
    {
        return AddFunction<TOut>(operation);
    }

    public IOperationBuilder<TIn, TOut> AddFunctionIf(Predicate<OperationContext<TIn>> predicate, OperationFunc<TIn, TOut> operation)
    {
        return AddOperation(async (context, updater) =>
        {
            if (!predicate(context)) return;
            var result = await operation(context);
            updater.Invoke(result);
        });

    }

    public IOperationBuilder<TIn, TOut> AddAction(OperationAction<TIn> operation)
    {
        return AddOperation((context, _) => operation.Invoke(context));
    }
    public IOperationBuilder<TIn, TOut> AddActionIf(Predicate<OperationContext<TIn>> predicate, OperationAction<TIn> operation)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(operation);
        return AddOperation((context, _) => !predicate(context) ? ValueTask.CompletedTask : operation.Invoke(context));
    }

    public IBuildableOperation<TIn, TR> AddFunction<TR>(OperationFunc<TIn, TR> operation)
    {

        return new BuildableOperation<TIn, TR>(InitFunc);

        async ValueTask<TR> InitFunc(OperationContext<TIn> context)
        {
            var currentInvoker = Compose();
            await currentInvoker.Invoke(context);
            return await operation.Invoke(context);
        }
    }

    public IOperationBuilder<TIn, TR> AddFunctionIf<TR>(Predicate<OperationContext<TIn>> predicate, OperationFunc<TIn, TR> operation)
    {
        return new OperationBuilder<TIn, TR>().AddOperation(async (context, updater) =>
        {
            if (!predicate(context)) return;
            var currentInvoker = Compose();
            await currentInvoker.Invoke(context);
            var result = await operation.Invoke(context);
            updater.Invoke(result);
        });
    }

    public IOperationBuilder<TIn, TOut> AddAction<T>() where T : IAction<TIn>
    {
        return AddAction(async context =>
        {
            var operation = context.ServiceProvider.GetRequiredService<T>();
            await operation.ExecuteAsync(context.Request, context.CancellationToken);
        });
    }

    public IOperationBuilder<TIn, TOut> AddActionIf<T>(Predicate<OperationContext<TIn>> predicate) where T : IAction<TIn>
    {
        return AddActionIf(predicate, async context =>
        {
            var operation = context.ServiceProvider.GetRequiredService<T>();
            await operation.ExecuteAsync(context.Request, context.CancellationToken);
        });
    }

    public IBuildableOperation<TIn, TOut> AddFunction<T>() where T : IFunction<TIn, TOut>
    {
        return AddFunction(context =>
        {
            var operation = context.ServiceProvider.GetRequiredService<T>();
            return operation.HandleAsync(context.Request, context.CancellationToken);
        });
    }

    public IOperationBuilder<TIn, TOut> AddFunctionIf<T>(Predicate<OperationContext<TIn>> predicate) where T : IFunction<TIn, TOut>
    {
        return AddFunctionIf(predicate, context =>
        {
            var operation = context.ServiceProvider.GetRequiredService<T>();
            return operation.HandleAsync(context.Request, context.CancellationToken);
        });
    }
    private OperationFunc<TIn, TOut> Compose()
    {
        Func<OperationContext<TIn, TOut>, Action<TOut>, ValueTask> invokeFunc = (_, _) => ValueTask.CompletedTask;
        invokeFunc = _operations.Aggregate(invokeFunc, (step, next) => async (c, resultUpdater) =>
        {
            await step.Invoke(c, resultUpdater);
            await next.Invoke(c, resultUpdater);
        });
        return async context =>
        {
            TOut result = default;
            await invokeFunc.Invoke(context.Map(result), r => result = r);
            return result;
        };
    }

}

