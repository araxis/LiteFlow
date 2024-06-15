// See https://aka.ms/new-console-template for more information

using LiteFlow;
using LiteFlow.Core;
using LiteFlow.Extensions;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("Hello, World!");
var services = new ServiceCollection();
var sp = services.BuildServiceProvider();
var builder = new FlowBuilder<Request, Response1>(sp)
        //.UseMiddleware(async (context, next) =>
        //    {
        //        Console.WriteLine(" before s1");
        //        var result = await next.Invoke();
        //        Console.WriteLine(" after s1");
        //        return result;

        //    })
        //.UseMiddleware(async (context, next) =>
        //    {
        //        Console.WriteLine("  before s2");
        //        var result = await next.Invoke();
        //        Console.WriteLine("  after s2");
        //        return result;
        //    })
        .Run(Config)

    ;

var flow = builder.Build();
var result = await flow.ExecuteAsync(new Request());
Console.WriteLine(result);
return;

OperationDelegate<Request, Response1> Config(IOperationBuilder<Request, Response1> chain)
{
    return chain
        .AddOperation(c=> ValueTask.FromResult(new Response3()))
        .AddOperation(c => Console.WriteLine("step1"))
        .AddOperation(c =>
        {
            Console.WriteLine("Set Result");
            return ValueTask.FromResult(new Response1(1));
        })
        .AddResultProcessor((_, _, _) =>
        {
            Console.WriteLine("Result Processor1");
        })
        .AddResultProcessor(_ =>
        {
            Console.WriteLine("Result Processor2");
        })
        .AddOperation(_ =>
        {
            Console.WriteLine("step2");
            return ValueTask.CompletedTask;
        })
        .AddOperation(c=>
        {
            Console.WriteLine("Response 2");
            return ValueTask.FromResult(new Response2());
        })
        .AddResultProcessor(c=> Console.WriteLine("Process Response2"))
        .AddOperation(c=>
        {
            Console.WriteLine("Back to response1");
            return ValueTask.FromResult(new Response1(2));
        })
        .Build();
}


public record Request;
public record Response1(int Id);
public record Response2;
public record Response3;
public record Result;