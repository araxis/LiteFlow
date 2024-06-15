namespace LiteFlow.Core;

public interface IOperationBuilder<TIn, TOut>
{
    IOperationBuilder<TIn, TOut> AddOperation(OperationDelegate<TIn> operation);

    IOperationBuilder<TIn, TOut> AddOperationIf(Predicate<OperationContext<TIn>> predicate, OperationDelegate<TIn> operation);

    IOperationBuilder<TIn, TOut> AddOperationIfElse(Predicate<OperationContext<TIn>> predicate, OperationDelegate<TIn> ifOperation, OperationDelegate<TIn> elseOperation);

    IBuildableOperation<TIn, TOut> AddOperation(OperationDelegate<TIn, TOut> operation);
    IBuildableOperation<TIn, TR> AddOperation<TR>(OperationDelegate<TIn, TR> operation);

    IBuildableOperation<TIn, TOut> AddOperationIf(Predicate<OperationContext<TIn>> predicate, OperationDelegate<TIn, TOut> operation);

    IBuildableOperation<TIn, TOut> AddOperationIfElse(Predicate<OperationContext<TIn>> predicate, OperationDelegate<TIn, TOut> ifOperation, OperationDelegate<TIn, TOut> elseOperation);

}