using LiteFlow.Core;
using Microsoft.Extensions.DependencyInjection;

namespace LiteFlow;

internal class BuildableOperation<TIn> : IBuildableOperation<TIn>
{
    private readonly List<OperationAction<TIn>> _operations;

    public BuildableOperation() : this([])
    {
    }
    internal BuildableOperation(IEnumerable<OperationAction<TIn>> operations)
    {
        ArgumentNullException.ThrowIfNull(operations);
        _operations = operations.ToList();
    }

    public IBuildableOperation<TIn> AddAction(OperationAction<TIn> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        _operations.Add(operation);
        return this;
    }

    public IBuildableOperation<TIn> AddActionIf(Predicate<OperationContext<TIn>> predicate, OperationAction<TIn> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        _operations.Add((context) =>
        {
            if (!predicate(context)) return ValueTask.CompletedTask;
            return operation.Invoke(context);
        });
        return this;
    }

    public IBuildableOperation<TIn> AddAction<T>() where T : IAction<TIn>
    {
        return AddAction(context =>
        {
            var action = context.ServiceProvider.GetRequiredService<T>();
            return action.ExecuteAsync(context.Request, context.CancellationToken);
        });
    }

    public IBuildableOperation<TIn> AddAction<T>(Predicate<OperationContext<TIn>> predicate) where T : IAction<TIn>
    {
        return AddActionIf(predicate, context =>
        {
            var action = context.ServiceProvider.GetRequiredService<T>();
            return action.ExecuteAsync(context.Request, context.CancellationToken);
        });
    }

    public IBuildableOperation<TIn, TOut> AddFunction<TOut>(OperationFunc<TIn, TOut> operation)
    {
        return new BuildableOperation<TIn, TOut>(InitFunc);

        async ValueTask<TOut> InitFunc(OperationContext<TIn> c)
        {
            var currentInvoker = Build();
            await currentInvoker.Invoke(c);
            return await operation.Invoke(c);
        }
    }
    public IOperationBuilder<TIn, TOut> AddFunctionIf<TOut>(Predicate<OperationContext<TIn>> predicate, OperationFunc<TIn, TOut> operation)
    {

        return new OperationBuilder<TIn, TOut>().AddFunctionIf<TOut>(predicate, async c =>
        {
            var currentInvoker = Build();
            await currentInvoker.Invoke(c);
            return await operation.Invoke(c);
        });
    }

    public IBuildableOperation<TIn, TR> AddFunction<T, TR>() where T : IFunction<TIn, TR>
    {
        return AddFunction(context =>
        {
            var action = context.ServiceProvider.GetRequiredService<T>();
            return action.HandleAsync(context.Request, context.CancellationToken);
        });
    }

    public IOperationBuilder<TIn, TR> AddFunctionIf<T, TR>(Predicate<OperationContext<TIn>> predicate) where T : IFunction<TIn, TR>
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



internal class BuildableOperation<TIn, TOut> : IBuildableOperation<TIn, TOut>
{
    private readonly List<Func<OperationContext<TIn>, Action<TOut>, Func<TOut>, ValueTask>> _operations = [];
    private readonly OperationFunc<TIn, TOut> _initFunc;
    internal BuildableOperation(OperationFunc<TIn, TOut> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        _initFunc = operation;
    }


    internal BuildableOperation<TIn, TOut> AddOperation(Func<OperationContext<TIn>, Action<TOut>, Func<TOut>, ValueTask> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        _operations.Add(operation);
        return this;
    }

    public IBuildableOperation<TIn, TOut> AddAction(OperationActionWithResult<TIn, TOut> operation)
    {
        return AddOperation((context, _, resolver) => operation.Invoke(context.Map(resolver)));
    }

    public IBuildableOperation<TIn, TOut> AddActionIf(Predicate<OperationContext<TIn, TOut>> predicate, OperationActionWithResult<TIn, TOut> operation)
    {
        return AddOperation((context, _, resolver) =>
        {
            var newContext = context.Map(resolver);
            if (!predicate(newContext)) return ValueTask.CompletedTask;
            return operation.Invoke(newContext);
        });
    }

    public IBuildableOperation<TIn, TOut> AddFunction(OperationFuncWithResult<TIn, TOut, TOut> operation)
    {
        AddOperation(async (context, stateUpdater, resolver) =>
        {
            var result = await operation.Invoke(context.Map(resolver));
            stateUpdater.Invoke(result);
        });
        return this;
    }

    public IBuildableOperation<TIn, TOut> AddFunctionIf(Predicate<OperationContext<TIn, TOut>> predicate, OperationFuncWithResult<TIn, TOut, TOut> operation)
    {
        AddOperation(async (context, stateUpdater, resolver) =>
        {
            var newContext = context.Map(resolver);
            if (!predicate(newContext)) return;
            var result = await operation.Invoke(newContext);
            stateUpdater.Invoke(result);
        });
        return this;
    }

    public IBuildableOperation<TIn, TR> AddFunction<TR>(OperationFuncWithResult<TIn, TOut, TR> operation)
    {
        return new BuildableOperation<TIn, TR>(InitFunc);

        async ValueTask<TR> InitFunc(OperationContext<TIn> context)
        {
            var currentInvoker = Build();
            var currentResult = await currentInvoker.Invoke(context);
            var newContext = new OperationContext<TIn, TOut>(context.Request, currentResult, context.ServiceProvider, context.CancellationToken);
            return await operation.Invoke(newContext);
        }
    }

    public IOperationBuilder<TIn, TR> AddFunctionIf<TR>(Predicate<OperationContext<TIn, TOut>> predicate, OperationFuncWithResult<TIn, TOut, TR> operation)
    {
        return new OperationBuilder<TIn, TR>().AddOperation(async (context, stateUpdater) =>
        {
            var currentInvoker = Build();
            var currentResult = await currentInvoker.Invoke(context);
            var newContext = new OperationContext<TIn, TOut>(context.Request, currentResult, context.ServiceProvider, context.CancellationToken);
            if (!predicate(newContext)) return;
            var result = await operation.Invoke(newContext);
            stateUpdater.Invoke(result);

        });
    }

    public IBuildableOperation<TIn, TOut> AddAction<T>() where T : IAction
    {
        return AddAction(context =>
        {
            var action = context.ServiceProvider.GetRequiredService<T>();
            return action switch
            {
                IAction<TIn> a => a.ExecuteAsync(context.Request, context.CancellationToken),
                IAction<TIn, TOut> b => b.ExecuteAsync(context.Request, context.Response, context.CancellationToken),
                _ => throw new InvalidOperationException()
            };
        });
    }

    public IBuildableOperation<TIn, TOut> AddActionIf<T>(Predicate<OperationContext<TIn, TOut>> predicate) where T : IAction
    {
        return AddAction(context =>
        {
            if (!predicate(context)) return ValueTask.CompletedTask;
            var action = context.ServiceProvider.GetRequiredService<T>();
            return action switch
            {
                IAction<TIn> a => a.ExecuteAsync(context.Request, context.CancellationToken),
                IAction<TIn, TOut> b => b.ExecuteAsync(context.Request, context.Response, context.CancellationToken),
                _ => throw new InvalidOperationException()
            };
        });
    }

    public IBuildableOperation<TIn, TOut> AddFunction<T>() where T : IFunctionBase<TIn, TOut>
    {
        return AddFunction(context =>
        {
            var action = context.ServiceProvider.GetRequiredService<T>();
            return action switch
            {
                IFunction<TIn, TOut, TOut> b => b.HandleAsync(context.Request, context.Response, context.CancellationToken),
                IFunction<TIn, TOut> a => a.HandleAsync(context.Request, context.CancellationToken),
                _ => throw new InvalidOperationException()
            };
        });
    }
    public IBuildableOperation<TIn, TOut> AddFunctionIf<T>(Predicate<OperationContext<TIn, TOut>> predicate) where T : IFunctionBase<TIn, TOut>
    {
        return AddFunctionIf(predicate, context =>
        {
            var action = context.ServiceProvider.GetRequiredService<T>();
            return action switch
            {
                IFunction<TIn, TOut, TOut> b => b.HandleAsync(context.Request, context.Response, context.CancellationToken),
                IFunction<TIn, TOut> a => a.HandleAsync(context.Request, context.CancellationToken),
                _ => throw new InvalidOperationException()
            };
        });
    }
    public IBuildableOperation<TIn, TR> AddFunction<T, TR>() where T : IFunctionBase<TIn, TR>
    {
        return AddFunction((OperationFuncWithResult<TIn, TOut, TR>)(context =>
        {
            var action = context.ServiceProvider.GetRequiredService<T>();
            return action switch
            {
                IFunction<TIn, TR> a => a.HandleAsync(context.Request, context.CancellationToken),
                IFunction<TIn, TOut, TR> b => b.HandleAsync(context.Request, context.Response, context.CancellationToken),
                _ => throw new InvalidOperationException()
            };
        }));
    }

    public IOperationBuilder<TIn, TR> AddFunction<T, TR>(Predicate<OperationContext<TIn, TOut>> predicate) where T : IFunctionBase<TIn, TR>
    {
        return AddFunctionIf(predicate, (OperationFuncWithResult<TIn, TOut, TR>)(context =>
        {
            var action = context.ServiceProvider.GetRequiredService<T>();
            return action switch
            {
                IFunction<TIn, TR> a => a.HandleAsync(context.Request, context.CancellationToken),
                IFunction<TIn, TOut, TR> b => b.HandleAsync(context.Request, context.Response, context.CancellationToken),
                _ => throw new InvalidOperationException()
            };
        }));
    }

    public OperationFunc<TIn, TOut> Build()
    {
        Func<OperationContext<TIn, TOut>, Action<TOut>, Func<TOut>, ValueTask> invokeFunc =
            async (context, stateUpdater, _) =>
            {
                var initResult = await _initFunc.Invoke(context);
                stateUpdater.Invoke(initResult);
            };
        invokeFunc = _operations.Aggregate(invokeFunc, (step, next) => async (c, resultUpdater, resolver) =>
        {
            await step.Invoke(c, resultUpdater, resolver);
            await next.Invoke(c, resultUpdater, resolver);
        });
        return async context =>
        {
            TOut result = default;
            await invokeFunc.Invoke(context.Map(result)!, r => result = r, () => result);
            return result!;
        };
    }

}