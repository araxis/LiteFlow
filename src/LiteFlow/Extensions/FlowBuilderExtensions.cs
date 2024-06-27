using LiteFlow.Core;

namespace LiteFlow.Extensions;

public static class FlowBuilderExtensions
{

    public static FlowBuilder<TIn, TOut> Run<TIn, TOut>(this IFlowBuilderProvider<TIn, TOut> builder, Func<OperationBag<TIn, TOut>, IOperationBuilder<TIn,TOut>> config)
    {
        ArgumentNullException.ThrowIfNull(config);
        var operationBuilder =  config.Invoke(new OperationBag<TIn, TOut>());
        return builder.Run(operationBuilder.Build());
    }

    public static FlowBuilder<TIn> Run<TIn>(this IFlowBuilderProvider<TIn> builder, Action<OperationBuilder<TIn>> config)
    {
        ArgumentNullException.ThrowIfNull(config);
        var chain = new OperationBuilder<TIn>();
        config.Invoke(chain);
        return builder.Run(chain.Build());
    }
}