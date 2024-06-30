using FluentAssertions;
using LiteFlow.Core;
using LiteFlow.UnitTests.Operations;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace LiteFlow.UnitTests;

public class OperationBuilderTests
{
    private OperationBuilder<int, string> _operationBuilder;
    private IServiceProvider _serviceProvider;

    [SetUp]
    public void SetUp()
    {
        _serviceProvider = Substitute.For<IServiceProvider>();
        _serviceProvider.GetService<Function1>().Returns(new Function1());
        _serviceProvider.GetService<Action1>().Returns(new Action1());
        _operationBuilder = new OperationBuilder<int, string>();
    }


    [Test]
    public void AddFunction_ShouldReturnBuildableOperation()
    {
        OperationFunc<int, string> func = ctx => ValueTask.FromResult("test");

       var result = _operationBuilder.AddFunction(func);

       result.Should().BeAssignableTo<IBuildableOperation<int, string>>();
    }

    [Test]
    public void AddFunctionIf_ShouldReturnOperationBuilder()
    {
        OperationFunc<int, string> func = ctx => ValueTask.FromResult("test");
        Predicate<OperationContext<int>> predicate = ctx => ctx.Request > 0;

        var result =_operationBuilder.AddFunctionIf(predicate, func);

        result.Should().BeAssignableTo<IOperationBuilder<int, string>>();
    }


    [Test]
    public void AddAction_ShouldReturnOperationBuilder()
    {
        OperationAction<int> action = ctx =>
        {
            ctx.Request.Should().Be(1);
            return ValueTask.CompletedTask;
        };

       var result = _operationBuilder.AddAction(action);

       result.Should().BeAssignableTo<IOperationBuilder<int, string>>();
    }

    [Test]
    public void AddActionIf_ShouldReturnOperationBuilder()
    {
        OperationAction<int> action = ctx =>
        {
            ctx.Request.Should().Be(1);
            return ValueTask.CompletedTask;
        };
        Predicate<OperationContext<int>> predicate = ctx => ctx.Request > 0;

       var result = _operationBuilder.AddActionIf(predicate, action);
       result.Should().BeAssignableTo<IOperationBuilder<int, string>>();
    }


    [Test]
    public void AddFunction_WithGeneric_ShouldReturnBuildableOperation()
    {

       var result = _operationBuilder.AddFunction<Function1>();

       result.Should().BeAssignableTo<IBuildableOperation<int, string>>();
    }

    [Test]
    public void AddFunctionIf_WithGeneric_ShouldReturnOperationBuilder()
    {
        var function = Substitute.For<IFunction<int, string>>();
        function.HandleAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(ValueTask.FromResult("test"));
        _serviceProvider.GetService<IFunction<int, string>>().Returns(function);
        Predicate<OperationContext<int>> predicate = ctx => ctx.Request > 0;

        var result =_operationBuilder.AddFunctionIf<IFunction<int, string>>(predicate);

        result.Should().BeAssignableTo<IOperationBuilder<int, string>>();
    }

    [Test]
    public void AddAction_WithGeneric_ShouldReturnOperationBuilder()
    {

        _serviceProvider.GetRequiredService<Action1>().Returns(new Action1());

        var result =_operationBuilder.AddAction<Action1>();

        result.Should().BeAssignableTo<IOperationBuilder<int, string>>();
    }

    [Test]
    public void AddActionIf_WithGeneric_ShouldReturnOperationBuilder()
    {
        _serviceProvider.GetService<Action1>().Returns(new Action1());
        Predicate<OperationContext<int>> predicate = ctx => ctx.Request > 0;

       var result = _operationBuilder.AddActionIf<IAction<int>>(predicate);

       result.Should().BeAssignableTo<IOperationBuilder<int, string>>();
    }
}