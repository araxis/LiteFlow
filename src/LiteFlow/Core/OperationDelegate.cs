namespace LiteFlow.Core;


public delegate ValueTask OperationAction<TIn>(OperationContext<TIn> context);
public delegate ValueTask OperationActionWithResult<TIn, TOut>(OperationContext<TIn, TOut> context);

public delegate ValueTask<TOut> OperationFunc<TIn, TOut>(OperationContext<TIn> context);
public delegate ValueTask<TOut> OperationFuncWithResult<TIn,TPrev, TOut>(OperationContext<TIn,TPrev> context);