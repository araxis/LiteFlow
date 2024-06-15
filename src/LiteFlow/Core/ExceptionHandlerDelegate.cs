namespace LiteFlow.Core;

public delegate ValueTask<bool> ExceptionHandlerDelegate<T>(OperationContext<T> context, Exception exception, CancellationToken cancellationToken);