/* Unmerged change from project 'LiteFlow (net6.0)'
Before:
using LiteFlow.Core;
After:
using LiteFlow;
using LiteFlow;
using LiteFlow.Core;
*/
namespace LiteFlow.Core;

public interface IFlowBuilder<TIn, TOut>
{
    IFlowBuilder<TIn, TOut> UseMiddleware(Func<OperationContext<TIn>, Func<ValueTask<TOut>>, ValueTask<TOut>> middleware);
    IBuildableFlow<TIn, TOut> Run(OperationDelegate<TIn, TOut> operation);
}