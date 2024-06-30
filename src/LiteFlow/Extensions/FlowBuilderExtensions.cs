using LiteFlow.Core;

namespace LiteFlow.Extensions;

public static class FlowBuilderExtensions
{

    public static IBuildableFlow<TIn, TOut> Run<TIn, TOut>(this IFlowBuilder<TIn, TOut> builder, Func<IOperationBuilder<TIn, TOut>, IBuildableOperation<TIn,TOut>> config)
    {
        ArgumentNullException.ThrowIfNull(config);
        var operationBuilder =  config.Invoke(new OperationBuilder<TIn, TOut>());
        return builder.Run(operationBuilder.Build());
    }

    public static IBuildableFlow<TIn> Run<TIn>(this IFlowBuilder<TIn> builder, Action<IBuildableOperation<TIn>> config)
    {
        ArgumentNullException.ThrowIfNull(config);
        var chain = new BuildableOperation<TIn>();
        config.Invoke(chain);
        return builder.Run(chain.Build());
    }
}