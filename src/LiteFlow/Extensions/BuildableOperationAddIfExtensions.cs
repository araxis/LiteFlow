using LiteFlow.Core;

namespace LiteFlow.Extensions;

public static class BuildableOperationAddIfExtensions
{
    public static IBuildableOperation<TIn, TOut> AddOperationIf<TIn, TOut>(this IBuildableOperation<TIn, TOut> builder, Predicate<TIn> predicate, Action<TIn, IServiceProvider, CancellationToken> action)
    {
        return builder.AddOperationIf(c => predicate(c.Request), context =>
        {
            action(context.Request, context.ServiceProvider, context.CancellationToken);
            return ValueTask.CompletedTask;
        });
    }

    public static IBuildableOperation<TIn, TOut> AddOperationIf<TIn, TOut>(this IBuildableOperation<TIn, TOut> builder, Predicate<TIn> predicate, Action<TIn, CancellationToken> action)
    {
        return builder.AddOperationIf(c => predicate(c.Request), context =>
        {
            action(context.Request, context.CancellationToken);
            return ValueTask.CompletedTask;
        });
    }

    public static IBuildableOperation<TIn, TOut> AddOperationIf<TIn, TOut>(this IBuildableOperation<TIn, TOut> builder, Predicate<TIn> predicate, Action<TIn> action)
    {
        return builder.AddOperationIf(c => predicate(c.Request), context =>
        {
            action(context.Request);
            return ValueTask.CompletedTask;
        });
    }

    public static IBuildableOperation<TIn, TOut> AddOperationIf<TIn, TOut>(this IBuildableOperation<TIn, TOut> builder, Predicate<TIn> predicate, Func<TIn, TOut> action)
    {
        return builder.AddOperationIf(predicate,  request =>
        {
           var result = action.Invoke(request);
           return ValueTask.FromResult(result);
        });
    }

    public static IBuildableOperation<TIn, TOut> AddOperationIf<TIn, TOut>(this IBuildableOperation<TIn, TOut> builder, Predicate<TIn> predicate, Func<TIn, ValueTask<TOut>> action)
        => builder.AddOperationIf(c => predicate(c.Request), async context => await action(context.Request));

    public static IBuildableOperation<TIn, TOut> AddOperationIf<TIn, TOut>(this IBuildableOperation<TIn, TOut> builder, Predicate<TIn> predicate, Func<TIn, CancellationToken, TOut> action)
    {
        return builder.AddOperationIf(predicate, (request,token) =>
        {
            var result = action(request, token);
            return ValueTask.FromResult(result);
        });
    }

    public static IBuildableOperation<TIn, TOut> AddOperationIf<TIn, TOut>(this IBuildableOperation<TIn, TOut> builder, Predicate<TIn> predicate, Func<TIn, CancellationToken, ValueTask<TOut>> action)
        => builder.AddOperationIf(c => predicate(c.Request), async context => await action(context.Request, context.CancellationToken));

    public static IBuildableOperation<TIn, TOut> AddOperationIf<TIn, TOut>(this IBuildableOperation<TIn, TOut> builder, Predicate<TIn> predicate, Func<TIn, IServiceProvider, CancellationToken, TOut> action)
    {
        return builder.AddOperationIf(predicate, (request, provider, token) =>
        {
            var result = action.Invoke(request, provider, token);
            return ValueTask.FromResult(result);
        });
    }

    public static IBuildableOperation<TIn, TOut> AddOperationIf<TIn, TOut>(this IBuildableOperation<TIn, TOut> builder, Predicate<TIn> predicate, Func<TIn, IServiceProvider, CancellationToken, ValueTask<TOut>> action)
        => builder.AddOperationIf(c => predicate(c.Request), async context => await action(context.Request, context.ServiceProvider, context.CancellationToken));
}