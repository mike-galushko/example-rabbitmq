using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using RabbitMQ.Example;
using RabbitMQ.Example.Options;

namespace RabbitMQ.WebAPI;

public static class Program
{
    const string MyCorsPolicy = "_myCorsPolicy";

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddCors(opt => opt.AddPolicy(name: MyCorsPolicy, policy =>
        {
            policy
                .WithOrigins("http://localhost:801")
                .AllowAnyHeader()
                .AllowAnyMethod();
        }));

        builder.AddServices();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseAuthorization();
        app.UseCors(MyCorsPolicy);
        app.MapControllers();


        // Run all RabbitMQ consumers.
        var queueOptions = app.Services.GetService<IOptions<QueueOptions>>();
        if (queueOptions == null)
            throw new ArgumentNullException(nameof(QueueOptions));
        var options = queueOptions.Value;

        using var simple = new SimpleConsumer(options);
        using var workerA = new WorkerQueueConsumer(options, 1);
        using var workerB = new WorkerQueueConsumer(options, 2);
        using var publishA = new PublishSubscribeConsumer(options, 1);
        using var publishB = new PublishSubscribeConsumer(options, 2);
        using var routingA = new RoutingConsumer(options, 1);
        using var routingB = new RoutingConsumer(options, 2);

        await QueueInitialization.EnsureAsync(options);
        await simple.StartListening();
        await workerA.StartListening();
        await workerB.StartListening();
        await publishA.StartListening();
        await publishB.StartListening();
        await routingA.StartListening();
        await routingB.StartListening();

        app.Run();
    }

    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<QueueOptions>(
            builder.Configuration.GetSection("rabbitmq"));

        builder.Services.AddScoped<SimpleProducer>();
        builder.Services.AddScoped<WorkerQueueProducer>();
        builder.Services.AddScoped<PublishSubscribeProducer>();
        builder.Services.AddScoped<RoutingProducer>();
    }
}
