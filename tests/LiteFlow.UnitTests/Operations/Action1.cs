using LiteFlow.Core;

namespace LiteFlow.UnitTests.Operations;

public class Action1 :IAction<int>
{
    public ValueTask ExecuteAsync(int input, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }
}