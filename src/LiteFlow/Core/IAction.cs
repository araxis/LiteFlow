namespace LiteFlow.Core;


public interface IAction;
public interface IAction<in TIn> : IAction
{
    ValueTask HandleAsync(TIn input, CancellationToken cancellationToken);
}
public interface IAction<in TIn, in TOut> : IAction
{
    ValueTask HandleAsync(TIn input, TOut result, CancellationToken cancellationToken);
}

