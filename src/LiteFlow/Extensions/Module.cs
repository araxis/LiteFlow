using LiteFlow.Core;
using Microsoft.Extensions.DependencyInjection;

namespace LiteFlow.Extensions;

public static class Module
{
    public static IServiceCollection AddFlow<TIn, TOut>(this IServiceCollection services,Func<FlowBuilder<TIn,TOut>,IBuildableFlow<TIn,TOut>> config)
    {
        services.AddSingleton(sp =>
        {
            var builder = new FlowBuilder<TIn, TOut>(sp);
            var buildableFlow =config.Invoke(builder);
            return buildableFlow.Build();
        });
        return services;
    }
}