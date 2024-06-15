using LiteFlow.Core;

namespace LiteFlow.Extensions;

public static class BuildableOperationResultExtensions
{
    public static IBuildableOperation<TIn, TOut> AddResultProcessor<TIn, TOut>(this IBuildableOperation<TIn, TOut> builder, Action<TIn, TOut, IServiceProvider, CancellationToken> action)
    {
        return builder.AddResultProcessor(context =>
        {
            action.Invoke(context.Request, context.Result, context.ServiceProvider, context.CancellationToken);
            return ValueTask.CompletedTask;
        });
    }

    public static IBuildableOperation<TIn, TOut> AddResultProcessor<TIn, TOut>(this IBuildableOperation<TIn, TOut> builder, Action<TIn, TOut> action)
    {
        return builder.AddResultProcessor(context =>
        {
            action.Invoke(context.Request, context.Result);
            return ValueTask.CompletedTask;
        });
    }

    public static IBuildableOperation<TIn, TOut> AddResultProcessor<TIn, TOut>(this IBuildableOperation<TIn, TOut> builder, Action<TOut, IServiceProvider, CancellationToken> action)
    {
        return builder.AddResultProcessor(context =>
         {
             action.Invoke(context.Result, context.ServiceProvider, context.CancellationToken);
             return ValueTask.CompletedTask;
         });
    }

    public static IBuildableOperation<TIn, TOut> AddResultProcessor<TIn, TOut>(this IBuildableOperation<TIn, TOut> builder, Action<TOut> action)
    {
        return builder.AddResultProcessor(context =>
        {
            action.Invoke(context.Result);
            return ValueTask.CompletedTask;
        });
    }

    public static IBuildableOperation<TIn, TOut> AddOperation<TIn, TOut>(this IBuildableOperation<TIn, TOut> builder, Func<TOut, IServiceProvider, CancellationToken, TOut> action)
    {
        return builder.AddResultProcessor(context =>
        {
            var result = action.Invoke(context.Result, context.ServiceProvider, context.CancellationToken);
            return ValueTask.FromResult(result);
        });
    }

    public static IBuildableOperation<TIn, TOut> AddOperation<TIn, TOut>(this IBuildableOperation<TIn, TOut> builder, Func<TOut,TOut> action)
    {
        return builder.AddResultProcessor(context =>
        {
            var result = action.Invoke(context.Result);
            return ValueTask.FromResult(result);
        });
    }

    public static IBuildableOperation<TIn, TOut> AddOperation<TIn, TOut>(this IBuildableOperation<TIn, TOut> builder, Func<TIn, TOut, IServiceProvider, CancellationToken, TOut> action)
    {
        return builder.AddResultProcessor(context =>
        {
            var result = action.Invoke(context.Request, context.Result, context.ServiceProvider, context.CancellationToken);
            return ValueTask.FromResult(result);
        });
    }

    public static IBuildableOperation<TIn, TOut> AddOperation<TIn, TOut>(this IBuildableOperation<TIn, TOut> builder, Func<TIn, TOut, TOut> action)
    {
        return builder.AddResultProcessor(context =>
        {
            var result = action.Invoke(context.Request, context.Result);
            return ValueTask.FromResult(result);
        });
    }

    public static IBuildableOperation<TIn, TOut> AddOperation<TIn, TOut>(this IBuildableOperation<TIn, TOut> builder, Func<TOut, IServiceProvider, CancellationToken, ValueTask<TOut>> action)
    {
        return builder.AddResultProcessor(context => action.Invoke(context.Result, context.ServiceProvider, context.CancellationToken));
    }

    public static IBuildableOperation<TIn, TOut> AddOperation<TIn, TOut>(this IBuildableOperation<TIn, TOut> builder, Func<TOut,ValueTask<TOut>> action)
    {
        return builder.AddResultProcessor(context => action.Invoke(context.Result));
    }

    public static IBuildableOperation<TIn, TOut> AddOperation<TIn, TOut>(this IBuildableOperation<TIn, TOut> builder, Func<TIn, TOut, IServiceProvider, CancellationToken, ValueTask<TOut>> action)
    {
        return builder.AddResultProcessor(context => action.Invoke(context.Request, context.Result, context.ServiceProvider, context.CancellationToken));
    }

    public static IBuildableOperation<TIn, TOut> AddOperation<TIn, TOut>(this IBuildableOperation<TIn, TOut> builder, Func<TIn, TOut, ValueTask<TOut>> action)
    {
        return builder.AddResultProcessor(context => action.Invoke(context.Request, context.Result));
    }
}