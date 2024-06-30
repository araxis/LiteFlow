namespace LiteFlow.Core;


public interface IAction;
public interface IAction<in TIn> : IAction
{
    ValueTask ExecuteAsync(TIn input, CancellationToken cancellationToken);
}
public interface IAction<in TIn, in TOut> : IAction
{
    ValueTask ExecuteAsync(TIn input, TOut result, CancellationToken cancellationToken);
}

