namespace LiteFlow.Core;

public interface IOperationBuilder<TIn>
{

    OperationAction<TIn> Build();
}

public interface IOperationBuilder<TIn,TOut>
{
    IBuildableOperation<TIn, TOut> AddFunction(OperationFunc<TIn, TOut> operation);
    IBuildableOperation<TIn, TR> AddFunction<TR>(OperationFunc<TIn, TR> operation);
    IBuildableOperation<TIn, TOut> AddFunction<T>() where T : IFunction<TIn, TOut>;

    IOperationBuilder<TIn, TOut> AddFunctionIf(Predicate<OperationContext<TIn>> predicate, OperationFunc<TIn, TOut> operation);
    IOperationBuilder<TIn, TOut> AddAction(OperationAction<TIn> operation);
    IOperationBuilder<TIn, TOut> AddActionIf(Predicate<OperationContext<TIn>> predicate, OperationAction<TIn> operation);
    IOperationBuilder<TIn, TR> AddFunctionIf<TR>(Predicate<OperationContext<TIn>> predicate, OperationFunc<TIn, TR> operation);
    IOperationBuilder<TIn, TOut> AddAction<T>() where T : IAction<TIn>;
    IOperationBuilder<TIn, TOut> AddActionIf<T>(Predicate<OperationContext<TIn>> predicate) where T : IAction<TIn>;
    IOperationBuilder<TIn, TOut> AddFunctionIf<T>(Predicate<OperationContext<TIn>> predicate) where T : IFunction<TIn, TOut>;

}