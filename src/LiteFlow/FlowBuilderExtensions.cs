using LiteFlow.Core;

namespace LiteFlow;

public static class FlowBuilderExtensions
{
    public static IBuildableFlow<TIn, TOut> Run<TIn, TOut>(this IFlowBuilder<TIn, TOut> builder, Func<IOperationBuilder<TIn, TOut>, OperationDelegate<TIn, TOut>> config)
    {
        ArgumentNullException.ThrowIfNull(config);
        var chain = new OperationBuilder<TIn, TOut>();
        var operation = config.Invoke(chain);
        return builder.Run(operation);
    }
}