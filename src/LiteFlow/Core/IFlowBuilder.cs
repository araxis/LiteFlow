namespace LiteFlow.Core;

public interface IFlowBuilder<in TIn,TOut>
{
    IFlow<TIn, TOut> Build();
}

public interface IFlowBuilder<in TIn>
{
    IFlow<TIn> Build();
}