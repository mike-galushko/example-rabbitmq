using Microsoft.Extensions.Options;
using RabbitMQ.Example;

namespace RabbitMQ.WebAPI
{
    public class Program
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
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseAuthorization();
            app.UseCors(MyCorsPolicy);
            app.MapControllers();

            // Ensure all ReabbitMQ queus are created.
            await QueueInitialization.EnsureAsync();

            // Run all RabbitMQ consumers.
            using var simple = new SimpleConsumer();
            await simple.StartListening();
            using var workerA = new WorkerQueueConsumer(1);
            using var workerB = new WorkerQueueConsumer(2);
            await workerA.StartListening();
            await workerB.StartListening();
            using var publishA = new PublishSubscribeConsumer(1);
            using var publishB = new PublishSubscribeConsumer(2);
            await publishA.StartListening();
            await publishB.StartListening();

            app.Run();
        }
    }
}
