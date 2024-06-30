namespace LiteFlow.Core;

public interface IBuildableOperation<TIn, TOut>
{
    IBuildableOperation<TIn, TOut> AddAction(OperationActionWithResult<TIn, TOut> operation);
    IBuildableOperation<TIn, TOut> AddActionIf(Predicate<OperationContext<TIn, TOut>> predicate, OperationActionWithResult<TIn, TOut> operation);
    IBuildableOperation<TIn, TOut> AddFunction(OperationFuncWithResult<TIn, TOut, TOut> operation);
    IBuildableOperation<TIn, TOut> AddFunctionIf(Predicate<OperationContext<TIn, TOut>> predicate, OperationFuncWithResult<TIn, TOut, TOut> operation);
    IBuildableOperation<TIn, TR> AddFunction<TR>(OperationFuncWithResult<TIn, TOut, TR> operation);
    IBuildableOperation<TIn, TOut> AddAction<T>() where T : IAction;
    IBuildableOperation<TIn, TOut> AddActionIf<T>(Predicate<OperationContext<TIn, TOut>> predicate) where T : IAction;
    IBuildableOperation<TIn, TOut> AddFunction<T>()where T : IFunctionBase<TIn, TOut>;
    IBuildableOperation<TIn, TOut> AddFunctionIf<T>(Predicate<OperationContext<TIn, TOut>> predicate) where T : IFunctionBase<TIn, TOut>;
    IBuildableOperation<TIn, TR> AddFunction<T, TR>() where T : IFunctionBase<TIn, TR>;

    IOperationBuilder<TIn, TR> AddFunction<T, TR>(Predicate<OperationContext<TIn, TOut>> predicate) where T : IFunctionBase<TIn, TR>;
    IOperationBuilder<TIn, TR> AddFunctionIf<TR>(Predicate<OperationContext<TIn, TOut>> predicate, OperationFuncWithResult<TIn, TOut, TR> operation);
    OperationFunc<TIn, TOut> Build();
}

public interface IBuildableOperation<TIn> 
{
    IBuildableOperation<TIn> AddAction(OperationAction<TIn> operation);
    IBuildableOperation<TIn> AddActionIf(Predicate<OperationContext<TIn>> predicate, OperationAction<TIn> operation);
    IBuildableOperation<TIn> AddAction<T>() where T : IAction<TIn>;
    IBuildableOperation<TIn> AddAction<T>(Predicate<OperationContext<TIn>> predicate) where T : IAction<TIn>;
    IBuildableOperation<TIn, TOut> AddFunction<TOut>(OperationFunc<TIn, TOut> operation);
    IOperationBuilder<TIn, TOut> AddFunctionIf<TOut>(Predicate<OperationContext<TIn>> predicate, OperationFunc<TIn, TOut> operation);
    IBuildableOperation<TIn, TR> AddFunction<T, TR>() where T : IFunction<TIn, TR>;
    IOperationBuilder<TIn, TR> AddFunctionIf<T, TR>(Predicate<OperationContext<TIn>> predicate) where T : IFunction<TIn, TR>;
    OperationAction<TIn> Build();
}