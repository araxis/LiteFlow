namespace LiteFlow.Core;

public delegate ValueTask OperationDelegate<TIn>(OperationContext<TIn> context);
public delegate ValueTask<TOut> OperationDelegate<TIn, TOut>(OperationContext<TIn> context);

