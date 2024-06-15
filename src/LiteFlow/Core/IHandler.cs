namespace LiteFlow.Core;

public interface IHandler<in T>
{
    ValueTask Handle(T input,CancellationToken cancellationToken);
}