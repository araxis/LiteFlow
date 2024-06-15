namespace LiteFlow.Core;

public interface IBuildableOperation<TIn, TOut>
{
    IBuildableOperation<TIn, TOut> AddOperation(OperationDelegate<TIn> operation);
    IBuildableOperation<TIn, TOut> AddResultProcessor(Func<OperationContext<TIn, TOut>, ValueTask> operation);
    IBuildableOperation<TIn, TOut> AddResultProcessor(Func<OperationContext<TIn, TOut>, ValueTask<TOut>> operation);
    IBuildableOperation<TIn, TOut> AddOperationIf(Predicate<OperationContext<TIn>> predicate, OperationDelegate<TIn> operation);
    IBuildableOperation<TIn, TOut> AddOperation(OperationDelegate<TIn, TOut> operation);
    IBuildableOperation<TIn, TR> AddOperation<TR>(OperationDelegate<TIn, TR> operation);
    IBuildableOperation<TIn, TOut> AddOperation(Predicate<OperationContext<TIn>> predicate, OperationDelegate<TIn, TOut> operation);
    OperationDelegate<TIn, TOut> Build();
    IBuildableOperation<TIn, TOut> Clone();
}