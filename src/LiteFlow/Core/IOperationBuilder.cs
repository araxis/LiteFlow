namespace LiteFlow.Core;

public interface IOperationBuilder<TIn>
{
    OperationAction<TIn> Build();
}

public interface IOperationBuilder<TIn,TOut>
{
    OperationFunc<TIn,TOut> Build();
}