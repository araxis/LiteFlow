using LiteFlow.Core;

namespace LiteFlow;

public class OperationBuilder<TIn, TOut> : IOperationBuilder<TIn, TOut>
{
    internal IEnumerable<Func<OperationContext<TIn>, Action<TOut>, Func<TOut>, ValueTask>> Operations => _operations;
    private readonly List<Func<OperationContext<TIn>, Action<TOut>, Func<TOut>, ValueTask>> _operations ;

    public OperationBuilder()
    {
        _operations = [];
    }

    public OperationBuilder(IEnumerable<Func<OperationContext<TIn>, Action<TOut>, Func<TOut>, ValueTask>> operations)
    {
        _operations = operations.ToList();
    }
    internal void AddOperation(Func<OperationContext<TIn>, Action<TOut>, Func<TOut>, ValueTask> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        _operations.Add(operation);
    }
    public IOperationBuilder<TIn, TOut> AddOperation(OperationDelegate<TIn> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        _operations.Add(async (context, _, _) =>
        {
            await operation.Invoke(context);
        });
        return this;
    }

    public IOperationBuilder<TIn, TOut> AddOperationIf(Predicate<OperationContext<TIn>> predicate, OperationDelegate<TIn> operation)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(operation);
        AddOperation(async (context, _, _) =>
        {
            if (!predicate(context)) return;
            await operation.Invoke(context);
        });
        return this;
    }

    public IOperationBuilder<TIn, TOut> AddOperationIfElse(Predicate<OperationContext<TIn>> predicate, OperationDelegate<TIn> ifOperation, OperationDelegate<TIn> elseOperation)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(ifOperation);
        ArgumentNullException.ThrowIfNull(elseOperation);
        AddOperation(async (context, _, _) =>
        {
            if (predicate(context))
            {
                await ifOperation.Invoke(context);
                return;
            }
            await elseOperation.Invoke(context);
        });
        return this;
    }

    public IBuildableOperation<TIn, TOut> AddOperation(OperationDelegate<TIn, TOut> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        AddOperation(async (context, updateResult, _) =>
        {
            var result = await operation.Invoke(context);
            updateResult.Invoke(result);
        });
        return new BuildableOperation<TIn, TOut>(this);
    }

    public IBuildableOperation<TIn, TR> AddOperation<TR>(OperationDelegate<TIn, TR> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        var clonedBuilder = new OperationBuilder<TIn, TOut>(_operations);
        var clonedBuildable = new BuildableOperation<TIn, TOut>(clonedBuilder);
        return new BuildableOperation<TIn, TR>().AddOperation(Operation());

        OperationDelegate<TIn, TR> Operation() => async context =>
        {
            var func = clonedBuildable.Build();
            await func.Invoke(context);
            return await operation.Invoke(context);
        };
    }

    public IBuildableOperation<TIn, TOut> AddOperationIf(Predicate<OperationContext<TIn>> predicate, OperationDelegate<TIn, TOut> operation)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(operation);
        AddOperation(async (context, updateResult, _) =>
        {
            if (!predicate(context)) return;
            var result = await operation.Invoke(context);
            updateResult.Invoke(result);
        });
        return new BuildableOperation<TIn, TOut>(this);
    }

    public IBuildableOperation<TIn, TOut> AddOperationIfElse(Predicate<OperationContext<TIn>> predicate, OperationDelegate<TIn, TOut> ifOperation, OperationDelegate<TIn, TOut> elseOperation)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(ifOperation);
        ArgumentNullException.ThrowIfNull(elseOperation);
        AddOperation(async (context, updateResult, _) =>
        {
            if (predicate(context))
            {
                var ifResult = await ifOperation.Invoke(context);
                updateResult.Invoke(ifResult);
                return;
            };
            var elseResult = await elseOperation.Invoke(context);
            updateResult.Invoke(elseResult);
        });
        return new BuildableOperation<TIn, TOut>(this);
    }

 
}