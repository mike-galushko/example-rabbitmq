using RabbitMQ.Client;
using System.Text;

namespace RabbitMQ.Example;

public class WorkerQueueProducer
{
    public async Task Send(string message)
    {
        var factory = new ConnectionFactory()
        {
            HostName = "gmike_example_rabbit",
            //Port = 207,
        };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        var bytes = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: QueueNames.Worker, body: bytes);
    }
}
