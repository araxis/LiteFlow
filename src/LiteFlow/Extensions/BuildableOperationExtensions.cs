using LiteFlow.Core;

namespace LiteFlow.Extensions;

public static class BuildableOperationExtensions
{
    public static IBuildableOperation<TIn, TOut> AddOperation<TIn, TOut>(this IBuildableOperation<TIn, TOut> builder, Action<TIn, IServiceProvider, CancellationToken> action)
    {
        return builder.AddOperation(context =>
        {
            action(context.Request, context.ServiceProvider, context.CancellationToken);
            return ValueTask.CompletedTask;
        });
    }

    public static IBuildableOperation<TIn, TOut> AddOperation<TIn, TOut>(this IBuildableOperation<TIn, TOut> chain, Action<TIn, CancellationToken> action)
    {
        return chain.AddOperation(context =>
        {
            action(context.Request, context.CancellationToken);
            return ValueTask.CompletedTask;
        });
    }

    public static IBuildableOperation<TIn, TOut> AddOperation<TIn, TOut>(this IBuildableOperation<TIn, TOut> chain, Action<TIn> action)
    {
        return chain.AddOperation(context =>
        {
            action(context.Request);
            return ValueTask.CompletedTask;
        });
    }

    public static IBuildableOperation<TIn, TOut> AddOperation<TIn, TOut>(this IBuildableOperation<TIn, TOut> chain, Func<TIn, TOut> action)
    {
        return chain.AddOperation(context =>
        {
            var result = action(context.Request);
            return ValueTask.FromResult(result);
        });
    }

    public static IBuildableOperation<TIn, TOut> AddOperation<TIn, TOut>(this IBuildableOperation<TIn, TOut> chain, Func<TIn, ValueTask<TOut>> action)
        => chain.AddOperation(async context => await action(context.Request));

    public static IBuildableOperation<TIn, TOut> AddOperation<TIn, TOut>(this IBuildableOperation<TIn, TOut> chain, Func<TIn, CancellationToken, TOut> action)
    {
        return chain.AddOperation(context =>
        {
            var result = action(context.Request, context.CancellationToken);
            return ValueTask.FromResult(result);
        });
    }

    public static IBuildableOperation<TIn, TOut> AddOperation<TIn, TOut>(this IBuildableOperation<TIn, TOut> chain, Func<TIn, CancellationToken, ValueTask<TOut>> action)
        => chain.AddOperation(async context => await action(context.Request, context.CancellationToken));

    public static IBuildableOperation<TIn, TOut> AddOperation<TIn, TOut>(this IBuildableOperation<TIn, TOut> chain, Func<TIn, IServiceProvider, CancellationToken, TOut> action)
    {
        return chain.AddOperation(context =>
        {
            var result = action(context.Request, context.ServiceProvider, context.CancellationToken);
            return ValueTask.FromResult(result);
        });
    }

    public static IBuildableOperation<TIn, TOut> AddOperation<TIn, TOut>(this IBuildableOperation<TIn, TOut> chain, Func<TIn, IServiceProvider, CancellationToken, ValueTask<TOut>> action)
        => chain.AddOperation(async context => await action(context.Request, context.ServiceProvider, context.CancellationToken));

}