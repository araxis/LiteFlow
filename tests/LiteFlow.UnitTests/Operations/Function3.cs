using LiteFlow.Core;

namespace LiteFlow.UnitTests.Operations;

public class Function3(string result) : IFunction<int,string, string>
{

    public ValueTask<string> HandleAsync(int input, string prevResult, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult($"{prevResult}+{result}");
    }
}