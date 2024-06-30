namespace LiteFlow.Core;

public interface IFlowBuilder<TIn, TOut>
{
    IFlowBuilder<TIn, TOut> UseMiddleware(Func<OperationContext<TIn>, Func<ValueTask<TOut>>, ValueTask<TOut>> middleware);
    IFlowBuilder<TIn, TOut> UseMiddleware<T>() where T : IMiddleware<TIn, TOut>;
    IBuildableFlow<TIn, TOut> Run(OperationFunc<TIn, TOut> operation);
    IBuildableFlow<TIn, TOut> Run<T>() where T : IFunction<TIn, TOut>;
}

public interface IFlowBuilder<TIn>
{
    IFlowBuilder<TIn> UseMiddleware(Func<OperationContext<TIn>, Func<ValueTask>, ValueTask> middleware);
    IFlowBuilder<TIn> UseMiddleware<T>() where T : IMiddleware<TIn>;
    IBuildableFlow<TIn> Run(OperationAction<TIn> operation);
    IBuildableFlow<TIn> Run<T>() where T : IAction<TIn>;
}