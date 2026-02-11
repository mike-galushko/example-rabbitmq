using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Example.Setup;
using RabbitMQ.Example.Options;
using System.Text;

namespace RabbitMQ.Example;

public class WorkerQueueProducer
{
    private QueueOptions options;

    public WorkerQueueProducer(IOptions<QueueOptions> options)
    {
        this.options = options.Value;
    }

    public async Task Send(string message)
    {
        var factory = options.ToConnectionFactory();
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        var bytes = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: QueueNames.Worker, body: bytes);
    }
}
