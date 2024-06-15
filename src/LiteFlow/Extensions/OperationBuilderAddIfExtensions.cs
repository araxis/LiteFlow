using LiteFlow.Core;

namespace LiteFlow.Extensions;

public static class OperationBuilderAddIfExtensions
{
    public static IOperationBuilder<TIn, TOut> AddOperationIf<TIn, TOut>(this IOperationBuilder<TIn, TOut> chain, Predicate<TIn> predicate, Action<TIn, IServiceProvider, CancellationToken> action)
    {
        return chain.AddOperationIf(c => predicate(c.Request), context =>
        {
            action(context.Request, context.ServiceProvider, context.CancellationToken);
            return ValueTask.CompletedTask;
        });
    }

    public static IOperationBuilder<TIn, TOut> AddOperationIf<TIn, TOut>(this IOperationBuilder<TIn, TOut> chain, Predicate<TIn> predicate, Action<TIn, CancellationToken> action)
    {
        return chain.AddOperationIf(c => predicate(c.Request), context =>
        {
            action(context.Request, context.CancellationToken);
            return ValueTask.CompletedTask;
        });
    }

    public static IOperationBuilder<TIn, TOut> AddOperationIf<TIn, TOut>(this IOperationBuilder<TIn, TOut> chain, Predicate<TIn> predicate, Action<TIn> action)
    {
        return chain.AddOperationIf(c => predicate(c.Request), context =>
        {
            action(context.Request);
            return ValueTask.CompletedTask;
        });
    }

    public static IBuildableOperation<TIn, TOut> AddOperationIf<TIn, TOut>(this IOperationBuilder<TIn, TOut> chain, Predicate<TIn> predicate, Func<TIn, TOut> action)
    {
        return chain.AddOperationIf(c => predicate(c.Request), context =>
        {
            var result = action(context.Request);
            return ValueTask.FromResult(result);
        });
    }

    public static IBuildableOperation<TIn, TOut> AddOperationIf<TIn, TOut>(this IOperationBuilder<TIn, TOut> chain, Predicate<TIn> predicate, Func<TIn, ValueTask<TOut>> action)
        => chain.AddOperationIf(c => predicate(c.Request), async context => await action(context.Request));

    public static IBuildableOperation<TIn, TOut> AddOperationIf<TIn, TOut>(this IOperationBuilder<TIn, TOut> chain, Predicate<TIn> predicate, Func<TIn, CancellationToken, TOut> action)
    {
        return chain.AddOperationIf(c => predicate(c.Request),context =>
        {
            var result = action(context.Request, context.CancellationToken);
            return ValueTask.FromResult(result);
        });
    }

    public static IBuildableOperation<TIn, TOut> AddOperationIf<TIn, TOut>(this IOperationBuilder<TIn, TOut> chain, Predicate<TIn> predicate, Func<TIn, CancellationToken, ValueTask<TOut>> action)
        => chain.AddOperationIf(c => predicate(c.Request), async context => await action(context.Request, context.CancellationToken));

    public static IBuildableOperation<TIn, TOut> AddOperationIf<TIn, TOut>(this IOperationBuilder<TIn, TOut> chain, Predicate<TIn> predicate, Func<TIn, IServiceProvider, CancellationToken, TOut> action)
    {
        return chain.AddOperationIf(c => predicate(c.Request),context =>
        {
            var result = action(context.Request, context.ServiceProvider, context.CancellationToken);
            return ValueTask.FromResult(result);
        });
    }

    public static IBuildableOperation<TIn, TOut> AddOperationIf<TIn, TOut>(this IOperationBuilder<TIn, TOut> chain, Predicate<TIn> predicate, Func<TIn, IServiceProvider, CancellationToken, ValueTask<TOut>> action)
        => chain.AddOperationIf(c => predicate(c.Request), async context => await action(context.Request, context.ServiceProvider, context.CancellationToken));
}