namespace LiteFlow.Core;

public interface IFlow<in TIn, TOut>
{
    ValueTask<TOut> ExecuteAsync(TIn input, CancellationToken cancellationToken = default);
}
public interface IFlow<in TIn>
{
    ValueTask ExecuteAsync(TIn input, CancellationToken cancellationToken = default);
}