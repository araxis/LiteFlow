using LiteFlow.Core;
using Microsoft.Extensions.DependencyInjection;

namespace LiteFlow;

public class OperationBuilderEx<TIn, TOut> : IOperationBuilder<TIn, TOut>
{
    private readonly List<Func<OperationContext<TIn>, Action<TOut>, Func<TOut>, ValueTask>> _operations;

    internal OperationBuilderEx() : this([])
    {
    }
    internal OperationBuilderEx(IEnumerable<Func<OperationContext<TIn>, Action<TOut>, Func<TOut>, ValueTask>> operations)
    {
        ArgumentNullException.ThrowIfNull(operations);
        _operations = operations.ToList();
    }

    internal OperationBuilderEx<TIn, TOut> AddOperation(Func<OperationContext<TIn>, Action<TOut>, Func<TOut>, ValueTask> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        _operations.Add(operation);
        return this;
    }

    public OperationBuilderEx<TIn, TOut> AddAction(OperationActionWithResult<TIn, TOut> operation)
    {
        return AddOperation((context, _, resolver) => operation.Invoke(context.Map(resolver)));
    }

    public OperationBuilderEx<TIn, TOut> AddActionIf(Predicate<OperationContext<TIn, TOut>> predicate, OperationActionWithResult<TIn, TOut> operation)
    {
        return AddOperation((context, _, resolver) =>
        {
            var newContext = context.Map(resolver);
            if (!predicate(newContext)) return ValueTask.CompletedTask;
            return operation.Invoke(newContext);
        });
    }


    public OperationBuilderEx<TIn, TOut> AddFunction(OperationFuncWithResult<TIn, TOut, TOut> operation)
    {
        _operations.Add(async (context, stateUpdater, resolver) =>
        {
            var result = await operation.Invoke(context.Map(resolver));
            stateUpdater.Invoke(result);
        });
        return this;
    }

    public OperationBuilderEx<TIn, TOut> AddFunctionIf(Predicate<OperationContext<TIn, TOut>> predicate, OperationFuncWithResult<TIn, TOut, TOut> operation)
    {
        _operations.Add(async (context, stateUpdater, resolver) =>
        {
            var newContext = context.Map(resolver);
            if (!predicate(newContext)) return;
            var result = await operation.Invoke(newContext);
            stateUpdater.Invoke(result);
        });
        return this;
    }

    public OperationBuilderEx<TIn, TR> AddFunction<TR>(OperationFuncWithResult<TIn, TOut, TR> operation)
    {
        return new OperationBuilderEx<TIn, TR>().AddOperation(async (context, stateUpdater,_) =>
        {
            var currentInvoker = Build();
            var currentResult = await currentInvoker.Invoke(context);
            var result = await operation.Invoke(new OperationContext<TIn, TOut>(context.Request, currentResult, context.ServiceProvider, context.CancellationToken));
            stateUpdater.Invoke(result);
        });
    }

    public OperationBuilderEx<TIn, TR> AddFunctionIf<TR>(Predicate<OperationContext<TIn, TOut>> predicate, OperationFuncWithResult<TIn, TOut, TR> operation)
    {
        return new OperationBuilderEx<TIn, TR>().AddOperation(async (context, stateUpdater,_) =>
        {
            var currentInvoker = Build();
            var currentResult = await currentInvoker.Invoke(context);
            var newContext = new OperationContext<TIn, TOut>(context.Request, currentResult, context.ServiceProvider, context.CancellationToken);
            if (!predicate(newContext)) return;
            var result = await operation.Invoke(newContext);
            stateUpdater.Invoke(result);

        });
    }

    public OperationBuilderEx<TIn, TOut> AddAction<T>() where T : IAction
    {
        return AddAction(context =>
        {
            var action = context.ServiceProvider.GetRequiredService<T>();
            return action switch
            {
                IAction<TIn> a => a.HandleAsync(context.Request, context.CancellationToken),
                IAction<TIn, TOut> b => b.HandleAsync(context.Request, context.Response, context.CancellationToken),
                _ => throw new InvalidOperationException()
            };
        });
    }

    public OperationBuilderEx<TIn, TOut> AddAction<T>(Predicate<OperationContext<TIn, TOut>> predicate) where T : IAction
    {
        return AddAction(context =>
        {
            if (!predicate(context)) return ValueTask.CompletedTask;
            var action = context.ServiceProvider.GetRequiredService<T>();
            return action switch
            {
                IAction<TIn> a => a.HandleAsync(context.Request, context.CancellationToken),
                IAction<TIn, TOut> b => b.HandleAsync(context.Request, context.Response, context.CancellationToken),
                _ => throw new InvalidOperationException()
            };
        });
    }

    public OperationBuilderEx<TIn, TOut> AddFunction<T>() where T : IFunctionBase<TIn, TOut>
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
    public OperationBuilderEx<TIn, TOut> AddFunction<T>(Predicate<OperationContext<TIn, TOut>> predicate) where T : IFunction<TIn, TOut>
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
    public OperationBuilderEx<TIn, TR> AddFunction<T, TR>() where T : IFunctionBase<TIn, TR>
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

    public OperationBuilderEx<TIn, TR> AddFunction<T, TR>(Predicate<OperationContext<TIn, TOut>> predicate) where T : IFunctionBase<TIn, TR>
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
        Func<OperationContext<TIn, TOut>, Action<TOut>, Func<TOut>, ValueTask> invokeFunc = (_, _, _) => ValueTask.CompletedTask;
        invokeFunc = _operations.Aggregate(invokeFunc, (step, next) => async (c, resultUpdater, resolver) =>
        {
            await step.Invoke(c, resultUpdater, resolver);
            await next.Invoke(c, resultUpdater, resolver);
        });
        return async context =>
        {
            TOut result = default;
            await invokeFunc.Invoke(context.Map(result), r => result = r, () => result);
            return result;
        };
    }

}