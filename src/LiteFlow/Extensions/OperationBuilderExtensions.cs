using LiteFlow.Core;

namespace LiteFlow.Extensions;

public static class OperationBuilderExtensions
{
    public static IOperationBuilder<TIn, TOut> AddOperation<TIn, TOut>(this IOperationBuilder<TIn, TOut> builder, Action<TIn, IServiceProvider, CancellationToken> action)
    {
        return builder.AddOperation(context =>
        {
            action(context.Request, context.ServiceProvider, context.CancellationToken);
            return ValueTask.CompletedTask;
        });
    }

    public static IOperationBuilder<TIn, TOut> AddOperation<TIn, TOut>(this IOperationBuilder<TIn, TOut> builder, Action<TIn, CancellationToken> action)
    {
        return builder.AddOperation(context =>
        {
            action(context.Request, context.CancellationToken);
            return ValueTask.CompletedTask;
        });
    }

    public static IOperationBuilder<TIn, TOut> AddOperation<TIn, TOut>(this IOperationBuilder<TIn, TOut> builder, Action<TIn> action)
    {
        return builder.AddOperation(context =>
        {
            action(context.Request);
            return ValueTask.CompletedTask;
        });
    }

    public static IBuildableOperation<TIn, TOut> AddOperation<TIn, TOut>(this IOperationBuilder<TIn, TOut> builder, Func<TIn, TOut> action)
    {
        return builder.AddOperation(context =>
        {
            var result = action(context.Request);
            return ValueTask.FromResult(result);
        });
    }

    public static IBuildableOperation<TIn, TOut> AddOperation<TIn, TOut>(this IOperationBuilder<TIn, TOut> builder, Func<TIn, ValueTask<TOut>> action)
        => builder.AddOperation(async context => await action(context.Request));

    public static IBuildableOperation<TIn, TOut> AddOperation<TIn, TOut>(this IOperationBuilder<TIn, TOut> builder, Func<TIn, CancellationToken, TOut> action)
    {
        return builder.AddOperation(context =>
        {
            var result = action(context.Request, context.CancellationToken);
            return ValueTask.FromResult(result);
        });
    }

    public static IBuildableOperation<TIn, TOut> AddOperation<TIn, TOut>(this IOperationBuilder<TIn, TOut> builder, Func<TIn, CancellationToken, ValueTask<TOut>> action)
        => builder.AddOperation(async context => await action(context.Request, context.CancellationToken));

    public static IBuildableOperation<TIn, TOut> AddOperation<TIn, TOut>(this IOperationBuilder<TIn, TOut> builder, Func<TIn, IServiceProvider, CancellationToken, TOut> action)
    {
        return builder.AddOperation(context =>
        {
            var result = action(context.Request, context.ServiceProvider, context.CancellationToken);
            return ValueTask.FromResult(result);
        });
    }

    public static IBuildableOperation<TIn, TOut> AddOperation<TIn, TOut>(this IOperationBuilder<TIn, TOut> builder, Func<TIn, IServiceProvider, CancellationToken, ValueTask<TOut>> action)
        => builder.AddOperation(async context => await action(context.Request, context.ServiceProvider, context.CancellationToken));

}