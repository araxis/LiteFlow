using LiteFlow.Core;

namespace LiteFlow.Builders;

public class OperationBuilder<TIn>
{

    private readonly List<OperationDelegate<TIn>> _operations;

    internal OperationBuilder()
    {
        _operations = [];
    }

    internal OperationBuilder(IEnumerable<OperationDelegate<TIn>> operations)
    {
        _operations = operations.ToList();
    }

    public OperationBuilder<TIn> AddActionStep<TStep>() where TStep : class, IStep<TIn>
    {
        return AddOperation(context =>
        {
            var handler = (TStep?)context.ServiceProvider.GetService(typeof(TStep));
            if (handler is null) throw new InvalidOperationException();
            return handler.Handle(context.Request, context.CancellationToken);

        });
    }

    public OperationBuilder<TIn> AddActionStep<TStep>(Predicate<OperationContext<TIn>> predicate) where TStep : class, IStep<TIn>
    {
        return AddOperation(predicate, context =>
        {
            var handler = (TStep?)context.ServiceProvider.GetService(typeof(TStep));
            if (handler is null) throw new InvalidOperationException();
            return handler.Handle(context.Request, context.CancellationToken);

        });
    }

    public OperationBuilder<TIn,TOut> AddFunctionStep<TStep,TOut>() where TStep : class, IStep<TIn,TOut>
    {
        return AddOperation(context =>
        {
            var handler = (TStep?)context.ServiceProvider.GetService(typeof(TStep));
            if (handler is null) throw new InvalidOperationException();
            return handler.Handle(context.Request, context.CancellationToken);

        });
    }

    public OperationBuilder<TIn,TOut> AddFunctionStep<TStep,TOut>(Predicate<OperationContext<TIn>> predicate) where TStep : class, IStep<TIn,TOut>
    {
        return AddOperation(predicate,context =>
        {
            var handler = (TStep?)context.ServiceProvider.GetService(typeof(TStep));
            if (handler is null) throw new InvalidOperationException();
            return handler.Handle(context.Request, context.CancellationToken);

        });
    }

    public OperationBuilder<TIn> AddOperation(OperationDelegate<TIn> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        _operations.Add(operation);
        return this;
    }
    public OperationBuilder<TIn> AddOperation(Predicate<OperationContext<TIn>> predicate, OperationDelegate<TIn> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        _operations.Add(c => !predicate(c) ? ValueTask.CompletedTask : operation.Invoke(c));
        return this;
    }

    public OperationBuilder<TIn, TOut> AddOperation<TOut>(OperationDelegate<TIn, TOut> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        var clonedBuilder = new OperationBuilder<TIn>(_operations);
        return new OperationBuilder<TIn, TOut>().AddOperation(c => clonedBuilder.Build().Invoke(c));
    }

    public OperationBuilder<TIn, TOut> AddOperation<TOut>(Predicate<OperationContext<TIn>> predicate, OperationDelegate<TIn, TOut> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        var clonedBuilder = new OperationBuilder<TIn>(_operations);
        return new OperationBuilder<TIn, TOut>().AddOperation(c => !predicate(c) ? ValueTask.CompletedTask : clonedBuilder.Build().Invoke(c));
    }

    OperationDelegate<TIn> Build() =>
        context =>
        {
            OperationDelegate<TIn> invokeFunc = _ => ValueTask.CompletedTask;
            invokeFunc = _operations.Aggregate(invokeFunc, (step, next) => async (c) =>
            {
                await step.Invoke(c);
                await next.Invoke(c);
            });
            return invokeFunc.Invoke(context);
        };
}

public class OperationBuilder<TIn, TOut>
{
    private readonly List<Func<OperationContext<TIn>, Action<TOut>, Func<TOut>, ValueTask>> _operations;

    internal OperationBuilder()
    {
        _operations = [];
    }

    internal OperationBuilder(IEnumerable<Func<OperationContext<TIn>, Action<TOut>, Func<TOut>, ValueTask>> operations)
    {
        _operations = operations.ToList();
    }

    internal OperationBuilder<TIn, TOut> AddOperation(Func<OperationContext<TIn>, Action<TOut>, Func<TOut>, ValueTask> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        _operations.Add(operation);
        return this;
    }

    public OperationBuilder<TIn, TOut> AddActionStep<TStep>() where TStep : class, IStep<TIn>
    {
        return AddOperation(context =>
        {
            var handler = (TStep?)context.ServiceProvider.GetService(typeof(TStep));
            if (handler is null) throw new InvalidOperationException();
            return handler.Handle(context.Request, context.CancellationToken);

        });
    }

    public OperationBuilder<TIn, TOut> AddActionStep<TStep>(Predicate<OperationContext<TIn, TOut>> predicate) where TStep : class, IStep<TIn>
    {
        return AddOperation(predicate, context =>
        {
            var handler = (TStep?)context.ServiceProvider.GetService(typeof(TStep));
            if (handler is null) throw new InvalidOperationException();
            return handler.Handle(context.Request, context.CancellationToken);

        });
    }
    public OperationBuilder<TIn, TOut> AddFunctionStep<TStep>() where TStep : class, IStep<TIn, TOut>
    {
        return AddOperation(context =>
        {
            var handler = (TStep?)context.ServiceProvider.GetService(typeof(TStep));
            if (handler is null) throw new InvalidOperationException();
            return handler.Handle(context.Request, context.CancellationToken);

        });
    }

    public OperationBuilder<TIn, TOut> AddFunctionStep<TStep>(Predicate<OperationContext<TIn, TOut>> predicate) where TStep : class, IStep<TIn, TOut>
    {
        return AddOperation(predicate, context =>
        {
            var handler = (TStep?)context.ServiceProvider.GetService(typeof(TStep));
            if (handler is null) throw new InvalidOperationException();
            return handler.Handle(context.Request, context.CancellationToken);
        });
    }

    public OperationBuilder<TIn, TOut> AddOperation(OperationDelegate<TIn> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        return AddOperation((context, _, _) => operation.Invoke(context));
    }

    public OperationBuilder<TIn, TOut> AddOperation(Predicate<OperationContext<TIn, TOut>> predicate, OperationDelegate<TIn> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        return AddOperation((context, _, resolver) =>
        {
            var opContext = new OperationContext<TIn, TOut>(context.Request, resolver.Invoke(), context.ServiceProvider, context.CancellationToken);
            return !predicate(opContext) ? ValueTask.CompletedTask : operation.Invoke(context);
        });
    }

    public OperationBuilder<TIn, TOut> AddOperation(OperationDelegate<TIn, TOut> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        return AddOperation(async (context, updater, _) =>
        {
            var result = await operation.Invoke(context);
            updater.Invoke(result);
        });
    }

    public OperationBuilder<TIn, TOut> AddOperation(Predicate<OperationContext<TIn, TOut>> predicate, OperationDelegate<TIn, TOut> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        return AddOperation(async (context, updater, resolver) =>
        {
            var opContext = new OperationContext<TIn, TOut>(context.Request, resolver.Invoke(), context.ServiceProvider, context.CancellationToken);
            if (!predicate(opContext)) return;
            var result = await operation.Invoke(context);
            updater.Invoke(result);
        });
    }

    public OperationBuilder<TIn, TR> AddOperation<TR>(OperationDelegate<TIn, TR> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        var clonedBuilder = new OperationBuilder<TIn, TOut>(_operations);
        return new OperationBuilder<TIn, TR>().AddOperation(async c =>
        {
            await clonedBuilder.Build().Invoke(c);
            return await operation.Invoke(c);
        });
    }

    public OperationBuilder<TIn, TR> AddOperation<TR>(OperationDelegate<TOut, TR> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        var clonedBuilder = new OperationBuilder<TIn, TOut>(_operations);
        return new OperationBuilder<TIn, TR>().AddOperation(async c =>
        {
           var result = await clonedBuilder.Build().Invoke(c);
            return await operation.Invoke(new OperationContext<TOut>(result,c.ServiceProvider,c.CancellationToken));
        });
    }

    public OperationBuilder<TIn, TR> AddOperation<TR>(Predicate<OperationContext<TIn, TOut>> predicate, OperationDelegate<TIn, TR> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        var clonedBuilder = new OperationBuilder<TIn, TOut>(_operations);
        return new OperationBuilder<TIn, TR>().AddOperation(async (c, updater, resolver) =>
        {
            var prevResult = await clonedBuilder.Build().Invoke(c);
            if (!predicate(new OperationContext<TIn, TOut>(c.Request, prevResult, c.ServiceProvider, c.CancellationToken))) return;
            var result = await operation.Invoke(c);
            updater.Invoke(result);
        });
    }

    OperationDelegate<TIn, TOut> Build()
    {
        return async context =>
        {
            TOut result = default;
            Func<OperationContext<TIn>, Action<TOut>, Func<TOut>, ValueTask> invokeFunc = (_, _, _) =>
                ValueTask.CompletedTask;
            invokeFunc = _operations.Aggregate(invokeFunc, (step, next) => async (c, resultUpdater, resultResolver) =>
            {
                await step.Invoke(c, resultUpdater, resultResolver);
                await next.Invoke(c, resultUpdater, resultResolver);
            });
            await invokeFunc.Invoke(context, r => result = r, () => result);
            return result;
        };
    }
}