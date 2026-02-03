using RabbitMQ.Client;
using RabbitMQ.Example.Options;

namespace RabbitMQ.Example;

public class QueueInitialization
{
    /// <summary>
    /// Ensure all ReabbitMQ queus are created.
    /// </summary>
    public static async Task EnsureAsync(QueueOptions options)
    {
        var factory = options.ToConnectionFactory();

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        // Simple queue
        await channel.QueueDeclareAsync(queue: QueueNames.Simple, durable: false, exclusive: false, autoDelete: false, arguments: null);
        
        // Worker queue
        await channel.QueueDeclareAsync(queue: QueueNames.Worker, durable: false, exclusive: false, autoDelete: false, arguments: null);
        
        // Publis/Subscribe queue
        await channel.QueueDeclareAsync(queue: QueueNames.PublishA, durable: false, exclusive: false, autoDelete: false, arguments: null);
        await channel.QueueDeclareAsync(queue: QueueNames.PublishB, durable: false, exclusive: false, autoDelete: false, arguments: null);
        await channel.ExchangeDeclareAsync(exchange: ExchangeNames.Publish, type: ExchangeType.Fanout);
        await channel.QueueBindAsync(queue: QueueNames.PublishA, exchange: ExchangeNames.Publish, routingKey: string.Empty);
        await channel.QueueBindAsync(queue: QueueNames.PublishB, exchange: ExchangeNames.Publish, routingKey: string.Empty);
    }
}
