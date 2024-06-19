namespace LiteFlow.Core;

public interface IStep;

public interface IStep<in TIn>
{
    ValueTask Handle(TIn input,CancellationToken cancellationToken);
}

public interface IStep<in TIn,TOut>
{
    ValueTask<TOut> Handle(TIn input,CancellationToken cancellationToken);
}
public interface IHandler<in T>
{
    ValueTask Handle(T input,CancellationToken cancellationToken);
}