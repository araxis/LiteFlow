namespace LiteFlow.Core;


public interface IMiddleware<TIn, TOut>
{
    ValueTask<TOut> InvokeAsync(OperationContext<TIn> context, OperationFunc<TIn,TOut> next);
}
public interface IMiddleware<TIn>
{
    ValueTask InvokeAsync(OperationContext<TIn> context, OperationAction<TIn> next);
}