using FluentAssertions;
using LiteFlow.Core;
using LiteFlow.UnitTests.Operations;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace LiteFlow.UnitTests;

public class BuildableOperationTests
{
    private IServiceProvider _serviceProvider;
    private OperationFunc<int, string> _initFunc;
    private BuildableOperation<int, string> _operationBuilder;

    [SetUp]
    public void SetUp()
    {
        _serviceProvider = Substitute.For<IServiceProvider>();
        _serviceProvider.GetService<Function1>().Returns(new Function1());
        _serviceProvider.GetService<Action1>().Returns(new Action1());
      
        _initFunc = ctx => ValueTask.FromResult("initial");
        _operationBuilder = new BuildableOperation<int, string>(_initFunc);
    }


    [Test]
    public void AddAction_ShouldReturnOperationBuilder()
    {
        OperationActionWithResult<int, string> action = ctx =>
        {
            ctx.Response.Should().Be("initial");
            return ValueTask.CompletedTask;
        };

        var result = _operationBuilder.AddAction(action);
        result.Should().BeAssignableTo<IOperationBuilder<int, string>>();
    }

    [Test]
    public void AddActionIf_ShouldReturnOperationBuilder()
    {
        OperationActionWithResult<int, string> action = ctx =>
        {
            ctx.Response.Should().Be("initial");
            return ValueTask.CompletedTask;
        };
        Predicate<OperationContext<int, string>> predicate = ctx => ctx.Request > 0;

        var result = _operationBuilder.AddActionIf(predicate, action);
        result.Should().BeAssignableTo<IOperationBuilder<int, string>>();
    }

    [Test]
    public void AddFunction_ShouldReturnOperationBuilder()
    {
        OperationFuncWithResult<int, string, string> func = ctx => ValueTask.FromResult("test");

        var result = _operationBuilder.AddFunction(func);
        result.Should().BeAssignableTo<IOperationBuilder<int, string>>();
    }

    [Test]
    public void AddFunctionIf_ShouldReturnOperationBuilder()
    {
        OperationFuncWithResult<int, string, string> func = ctx => ValueTask.FromResult("test");
        Predicate<OperationContext<int, string>> predicate = ctx => ctx.Request > 0;
        var result = _operationBuilder.AddFunctionIf(predicate, func);
        result.Should().BeAssignableTo<IOperationBuilder<int, string>>();
    }

    [Test]
    public void AddFunction_WithGeneric_ShouldReturnOperationBuilder()
    {
        var function = new Function1();
        _serviceProvider.GetService<Function1>().Returns(function);
        var result = _operationBuilder.AddFunction<Function1>();
        result.Should().BeAssignableTo<IOperationBuilder<int, string>>();
    }

    [Test]
    public void AddAction_WithGeneric_ShouldReturnOperationBuilder()
    {
        var action = _serviceProvider.GetRequiredService<Action1>();
        var result = _operationBuilder.AddAction<Action1>();

        result.Should().BeAssignableTo<IOperationBuilder<int, string>>();
    }

    [Test]
    public void AddActionIf_WithGeneric_ShouldReturnOperationBuilder()
    {
        Predicate<OperationContext<int, string>> predicate = ctx => ctx.Request > 0;
        var result = _operationBuilder.AddActionIf<Action1>(predicate);
        result.Should().BeAssignableTo<IOperationBuilder<int, string>>();;
    }


    [Test]
    public async Task Build_ShouldComposeAllOperations()
    {
        var initFunc = Substitute.For<OperationFunc<int, string>>();
        initFunc.Invoke(Arg.Any<OperationContext<int>>()).Returns(ValueTask.FromResult("initResult"));
        var builder = new BuildableOperation<int, string>(initFunc);
        var serviceProvider = Substitute.For<IServiceProvider>();
        var step1Result = "step1";
        var step2Result = "step2";
        serviceProvider.GetService<Function2>().Returns(new Function2(step1Result));
        serviceProvider.GetService<Function3>().Returns(new Function3(step2Result));
        var context = new OperationContext<int>(1, serviceProvider, CancellationToken.None);
        builder.AddFunction<Function2>()
            .AddFunction<Function3>();

        var result = await builder.Build().Invoke(context);

        result.Should().Be($"{step1Result}+{step2Result}");
    }
}