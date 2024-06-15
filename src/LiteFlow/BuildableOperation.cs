using LiteFlow.Core;

namespace LiteFlow;

internal class BuildableOperation<TIn, TOut>(OperationBuilder<TIn, TOut> builder) : IBuildableOperation<TIn, TOut>
{
    private readonly OperationBuilder<TIn, TOut> _builder = builder ?? throw new ArgumentNullException(nameof(builder));
    public BuildableOperation() : this(new OperationBuilder<TIn, TOut>())
    {
    }


    public IBuildableOperation<TIn, TOut> AddOperation(OperationDelegate<TIn> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        _builder.AddOperation(operation);
        return this;
    }


    public IBuildableOperation<TIn, TOut> AddResultProcessor(Func<OperationContext<TIn, TOut>, ValueTask> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        _builder.AddOperation(async (context, _, resultResolver) =>
        {
            var currentResult = resultResolver.Invoke();
            await operation.Invoke(new OperationContext<TIn, TOut>(context.Request, currentResult, context.ServiceProvider, context.CancellationToken));
        });
        return this;
    }

    public IBuildableOperation<TIn, TOut> AddResultProcessor(Func<OperationContext<TIn, TOut>, ValueTask<TOut>> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        _builder.AddOperation(async (context, resultUpdater, resultResolver) =>
        {
            var currentResult = resultResolver.Invoke();
            var newResult = await operation.Invoke(new OperationContext<TIn, TOut>(context.Request, currentResult, context.ServiceProvider, context.CancellationToken));
            resultUpdater.Invoke(newResult);
        });
        return this;
    }

    public IBuildableOperation<TIn, TOut> AddOperationIf(Predicate<OperationContext<TIn>> predicate, OperationDelegate<TIn> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(predicate);
        _builder.AddOperationIf(predicate, operation);
        return this;
    }

    public IBuildableOperation<TIn, TOut> AddOperation(OperationDelegate<TIn, TOut> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        _builder.AddOperation(operation);
        return this;
    }

    public IBuildableOperation<TIn, TR> AddOperation<TR>(OperationDelegate<TIn, TR> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        var newBuilder = new BuildableOperation<TIn, TR>();
        var clonedBuilder = Clone();
        return newBuilder.AddOperation(Operation());

        OperationDelegate<TIn, TR> Operation() => async context =>
        {
            var func = clonedBuilder.Build();
            await func.Invoke(context);
            return await operation.Invoke(context);
        };
    }
    public IBuildableOperation<TIn, TOut> AddOperation(Predicate<OperationContext<TIn>> predicate, OperationDelegate<TIn, TOut> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(predicate);
        _builder.AddOperationIf(predicate, operation);
        return this;
    }


    public OperationDelegate<TIn, TOut> Build()
    {

        return async context =>
        {
            TOut result = default;
            Func<OperationContext<TIn>, Action<TOut>, Func<TOut>, ValueTask> invokeFunc = (_, _, _) => ValueTask.CompletedTask;
            invokeFunc = _builder.Operations.Aggregate(invokeFunc, (step, next) => async (c, resultUpdater, resultResolver) =>
            {
                await step.Invoke(c, resultUpdater, resultResolver);
                await next.Invoke(c, resultUpdater, resultResolver);
            });
            await invokeFunc.Invoke(context, r => result = r, () => result);
            return result;
        };
    }

    public IBuildableOperation<TIn, TOut> Clone()
    {
        return new BuildableOperation<TIn, TOut>(new OperationBuilder<TIn, TOut>(_builder.Operations));
    }
}