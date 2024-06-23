using LiteFlow.Core;
using Microsoft.Extensions.DependencyInjection;

namespace LiteFlow;

public class OperationBuilder<TIn, TOut> 
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

    public OperationBuilderEx<TIn, TOut> AddFunction(OperationFunc<TIn, TOut> operation)
    {
        return AddFunction<TOut>(operation);
    }

    public OperationBuilder<TIn, TOut> AddFunctionIf(Predicate<OperationContext<TIn>> predicate, OperationFunc<TIn, TOut> operation)
    {
         _operations.Add(async (context,updater) =>
         {
             if(!predicate(context)) return;
             var result = await operation(context);
             updater.Invoke(result);
         });
         return this;
    }

    public OperationBuilder<TIn, TOut> AddAction(OperationAction<TIn> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        _operations.Add((context, _) => operation.Invoke(context));
        return this;
    }
    public OperationBuilder<TIn, TOut> AddActionIf(Predicate<OperationContext<TIn>> predicate, OperationAction<TIn> operation)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(operation);
         _operations.Add((context, _) =>
        {
            if (!predicate(context)) return ValueTask.CompletedTask;
            return operation.Invoke(context);
        });
         return this;
    }

    public OperationBuilderEx<TIn, TR> AddFunction<TR>(OperationFunc<TIn, TR> operation)
    {
        return new OperationBuilderEx<TIn, TR>()
            .AddFunction(async context =>
            {
                var currentInvoker = Build();
                var currentResult = await currentInvoker.Invoke(context);
                return await operation.Invoke(new OperationContext<TIn, TOut>(context.Request, currentResult, context.ServiceProvider, context.CancellationToken));
            });
    }

    public OperationBuilder<TIn, TR> AddFunctionIf<TR>(Predicate<OperationContext<TIn>> predicate, OperationFunc<TIn, TR> operation)
    {
        var currentInvoker = Build();
        return new OperationBuilder<TIn, TR>().AddOperation(async (context, updater) =>
        {
            if(!predicate(context)) return;
            var result = await operation.Invoke(context);
            updater.Invoke(result);
        });
    }

    public OperationBuilder<TIn, TOut> AddAction<T>() where T : IAction<TIn>
    {
        return AddAction(async context =>
        {
            var operation = context.ServiceProvider.GetRequiredService<T>();
            await operation.HandleAsync(context.Request, context.CancellationToken);
        });
    }

    public OperationBuilder<TIn, TOut> AddActionIf<T>(Predicate<OperationContext<TIn>> predicate) where T : IAction<TIn>
    {
        return AddActionIf(predicate, async context =>
        {
            var operation = context.ServiceProvider.GetRequiredService<T>();
            await operation.HandleAsync(context.Request, context.CancellationToken);
        });
    }

    public OperationBuilderEx<TIn, TOut> AddFunction<T>() where T : IFunction<TIn, TOut>
    {
        return AddFunction(context =>
        {
            var operation = context.ServiceProvider.GetRequiredService<T>();
            return operation.HandleAsync(context.Request, context.CancellationToken);
        });
    }

    public OperationBuilder<TIn, TOut> AddFunctionIf<T>(Predicate<OperationContext<TIn>> predicate) where T : IFunction<TIn, TOut>
    {
        return AddFunctionIf(predicate, context =>
        {
            var operation = context.ServiceProvider.GetRequiredService<T>();
            return operation.HandleAsync(context.Request, context.CancellationToken);
        });
    }

    public OperationFunc<TIn, TOut> Build()
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

public class OperationBuilder<TIn> : IOperationBuilder<TIn>
{
    private readonly List<OperationAction<TIn>> _operations;

    internal OperationBuilder() : this([])
    {
    }
    internal OperationBuilder(IEnumerable<OperationAction<TIn>> operations)
    {
        ArgumentNullException.ThrowIfNull(operations);
        _operations = operations.ToList();
    }

    public OperationBuilder<TIn> AddAction(OperationAction<TIn> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        _operations.Add(operation);
        return this;
    }

    public OperationBuilder<TIn> AddActionIf(Predicate<OperationContext<TIn>> predicate, OperationAction<TIn> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        _operations.Add((context) =>
        {
            if (!predicate(context)) return ValueTask.CompletedTask;
            return operation.Invoke(context);
        });
        return this;
    }

    public OperationBuilder<TIn> AddAction<T>() where T : IAction<TIn>
    {
        return AddAction(context =>
        {
            var action = context.ServiceProvider.GetRequiredService<T>();
            return action.HandleAsync(context.Request, context.CancellationToken);
        });
    }

    public OperationBuilder<TIn> AddAction<T>(Predicate<OperationContext<TIn>> predicate) where T : IAction<TIn>
    {
        return AddActionIf(predicate, context =>
        {
            var action = context.ServiceProvider.GetRequiredService<T>();
            return action.HandleAsync(context.Request, context.CancellationToken);
        });
    }

    public OperationBuilderEx<TIn, TOut> AddFunction<TOut>(OperationFunc<TIn, TOut> operation)
    {
        var currentInvoker = Build();
        return new OperationBuilderEx<TIn, TOut>().AddFunction(async c =>
        {
            await currentInvoker.Invoke(c);
            return await operation.Invoke(c);
        });
    }
    public OperationBuilderEx<TIn, TOut> AddFunctionIf<TOut>(Predicate<OperationContext<TIn>> predicate, OperationFunc<TIn, TOut> operation)
    {
        var currentInvoker = Build();
        return new OperationBuilderEx<TIn, TOut>().AddFunctionIf<TOut>(predicate, async c =>
        {
            await currentInvoker.Invoke(c);
            return await operation.Invoke(c);
        });
    }

    public OperationBuilderEx<TIn, TR> AddFunction<T, TR>() where T : IFunction<TIn, TR>
    {
        return AddFunction(context =>
        {
            var action = context.ServiceProvider.GetRequiredService<T>();
            return action.HandleAsync(context.Request, context.CancellationToken);
        });
    }

    public OperationBuilderEx<TIn, TR> AddFunction<T, TR>(Predicate<OperationContext<TIn>> predicate) where T : IFunction<TIn, TR>
    {
        return AddFunctionIf(predicate, context =>
        {
            var action = context.ServiceProvider.GetRequiredService<T>();
            return action.HandleAsync(context.Request, context.CancellationToken);
        });
    }

    public OperationAction<TIn> Build()
    {
        Func<OperationContext<TIn>, ValueTask> invokeFunc = (_) => ValueTask.CompletedTask;
        invokeFunc = _operations.Aggregate(invokeFunc, (step, next) => async (c) =>
        {
            await step.Invoke(c);
            await next.Invoke(c);
        });
        return invokeFunc.Invoke;
    }
}