using RabbitMQ.Client;

namespace RabbitMQ.Example;

public class QueueInitialization
{
    public static async Task EnsureAsync()
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            Port = 207,
        };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        // Simple queue
        await channel.QueueDeclareAsync(queue: QueueNames.Simple, durable: false, exclusive: false, autoDelete: false, arguments: null);
        // Worker queue
        await channel.QueueDeclareAsync(queue: QueueNames.Worker, durable: false, exclusive: false, autoDelete: false, arguments: null);
    }
}
