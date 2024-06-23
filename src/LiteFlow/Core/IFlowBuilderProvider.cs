namespace LiteFlow.Core;

public interface IFlowBuilderProvider<TIn, TOut>
{
    IFlowBuilderProvider<TIn, TOut> Use(Func<OperationContext<TIn>, Func<ValueTask<TOut>>, ValueTask<TOut>> middleware);
    IFlowBuilderProvider<TIn, TOut> Use<T>() where T : IMiddleware<TIn, TOut>;
    FlowBuilder<TIn, TOut> Run(OperationFunc<TIn, TOut> operation);
    FlowBuilder<TIn, TOut> Run<T>() where T : IFunction<TIn, TOut>;
}

public interface IFlowBuilderProvider<TIn>
{
    IFlowBuilderProvider<TIn> UseMiddleware(Func<OperationContext<TIn>, Func<ValueTask>, ValueTask> middleware);
    IFlowBuilderProvider<TIn> Use<T>() where T : IMiddleware<TIn>;
    FlowBuilder<TIn> Run(OperationAction<TIn> operation);
    FlowBuilder<TIn> Run<T>() where T : IAction<TIn>;
}