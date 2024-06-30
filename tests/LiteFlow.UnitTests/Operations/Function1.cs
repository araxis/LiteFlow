using LiteFlow.Core;

namespace LiteFlow.UnitTests.Operations;

public class Function1:IFunction<int,string>
{
    public ValueTask<string> HandleAsync(int input, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult("test");
    }
}