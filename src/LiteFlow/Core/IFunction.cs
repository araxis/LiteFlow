namespace LiteFlow.Core;

public interface IFunctionBase<in TIn, TOut>;
public interface IFunction<in TIn, TOut> : IFunctionBase<TIn, TOut>
{
    ValueTask<TOut> HandleAsync(TIn input, CancellationToken cancellationToken);
}
public interface IFunction<in TIn, in TPrevOut, TOut> : IFunctionBase<TIn, TOut>
{
    ValueTask<TOut> HandleAsync(TIn input, TPrevOut result, CancellationToken cancellationToken);
}