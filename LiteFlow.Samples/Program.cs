// See https://aka.ms/new-console-template for more information

using LiteFlow;
using LiteFlow.Core;
using Microsoft.Extensions.DependencyInjection;
using LiteFlow.Extensions;

Console.WriteLine("Hello, World!");
var services = new ServiceCollection();
services.AddSingleton<Handler1>();
services.AddSingleton<Handler2>();
services.AddSingleton<Handler3>();
var sp = services.BuildServiceProvider();
var builder = new FlowBuilderProvider<Request, Response1>(sp)
    .Use(async (context, next) =>
    {
        Console.WriteLine(" before s1");
        var result = await next.Invoke();
        Console.WriteLine(" after s1");
        return result;

    })
    .Use(async (context, next) =>
    {
        Console.WriteLine("  before s2");
        var result = await next.Invoke();
        Console.WriteLine("  after s2");
        return result;
    })
    .Run(b =>b
        .AddFunction(c=> ValueTask.FromResult(new Response1(1)))
        .AddFunction((context => ValueTask.FromResult(new Response1(context.Response.Id+1))))
        .AddAction<Handler1>()
        .AddFunction<Handler2>()
        .AddFunction<Handler3>()
        )
    ;

var flow = builder.Build();
var result1 = await flow.ExecuteAsync(new Request());
Console.WriteLine(result1);
return;

Response1 Do(Response1 r)
{
    return new Response1(r.Id + 1);
}

public class Handler1 : IAction<Request>
{
    public ValueTask HandleAsync(Request input, CancellationToken cancellationToken)
    {
        Console.WriteLine("Handler1");
        return ValueTask.CompletedTask;
    }
}
public class Handler2 : IFunction<Request,Response1, Response1>
{
    public ValueTask<Response1> HandleAsync(Request input, Response1 result, CancellationToken cancellationToken)
    {
        Console.WriteLine("Handler2");
        return ValueTask.FromResult(new Response1(Id: result.Id + 1));
    }
}
public class Handler3 : IFunction<Request,Response1, Response1>
{
    public ValueTask<Response1> HandleAsync(Request input, Response1 result, CancellationToken cancellationToken)
    {
        Console.WriteLine("Handler3");
        return ValueTask.FromResult(new Response1(Id: result.Id + 2));
    }
}
public record Request;
public record Response1(int Id);
public record Response2;
public record Response3;
public record Result;