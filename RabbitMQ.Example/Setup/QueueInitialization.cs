using RabbitMQ.Client;
using RabbitMQ.Example.Options;
using System.Security.AccessControl;

namespace RabbitMQ.Example.Setup;

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
        await channel.ExchangeDeclareAsync(exchange: ExchangeNames.Publish, type: ExchangeType.Fanout);
        await channel.QueueDeclareAsync(queue: QueueNames.PublishA, durable: false, exclusive: false, autoDelete: false, arguments: null);
        await channel.QueueDeclareAsync(queue: QueueNames.PublishB, durable: false, exclusive: false, autoDelete: false, arguments: null);
        await channel.QueueBindAsync(queue: QueueNames.PublishA, exchange: ExchangeNames.Publish, routingKey: string.Empty);
        await channel.QueueBindAsync(queue: QueueNames.PublishB, exchange: ExchangeNames.Publish, routingKey: string.Empty);

        // Routing queue
        await channel.ExchangeDeclareAsync(exchange: ExchangeNames.Routing, type: ExchangeType.Direct);
        await channel.QueueDeclareAsync(queue: QueueNames.RoutingA, durable: false, exclusive: false, autoDelete: false, arguments: null);
        await channel.QueueDeclareAsync(queue: QueueNames.RoutingB, durable: false, exclusive: false, autoDelete: false, arguments: null);
        await channel.QueueBindAsync(queue: QueueNames.RoutingA, exchange: ExchangeNames.Routing, routingKey: RoutingKeys.RoutingA);
        await channel.QueueBindAsync(queue: QueueNames.RoutingB, exchange: ExchangeNames.Routing, routingKey: RoutingKeys.RoutingB);

        // Topic queue
        await channel.ExchangeDeclareAsync(exchange: ExchangeNames.Topic, type: ExchangeType.Topic);
        await channel.QueueDeclareAsync(queue: QueueNames.TopicA, durable: false, exclusive: false, autoDelete: false, arguments: null);
        await channel.QueueDeclareAsync(queue: QueueNames.TopicB, durable: false, exclusive: false, autoDelete: false, arguments: null);
        await channel.QueueBindAsync(queue: QueueNames.TopicA, exchange: ExchangeNames.Topic, routingKey: RoutingKeys.TopicA);
        await channel.QueueBindAsync(queue: QueueNames.TopicA, exchange: ExchangeNames.Topic, routingKey: RoutingKeys.TopicB);

        // RPC queue
        await channel.QueueDeclareAsync(queue: QueueNames.RpcRequest, durable: false, exclusive: false, autoDelete: false, arguments: null);
        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);
    }

    public static void Reset()
    {
        SimpleConsumer.ReceivedMessages = string.Empty;
        WorkerQueueConsumer.ReceivedMessagesA = string.Empty;
        WorkerQueueConsumer.ReceivedMessagesB = string.Empty;
        PublishSubscribeConsumer.ReceivedMessagesA = string.Empty;
        PublishSubscribeConsumer.ReceivedMessagesB = string.Empty;
        RoutingConsumer.ReceivedMessagesA = string.Empty;
        RoutingConsumer.ReceivedMessagesB = string.Empty;
        TopicConsumer.ReceivedMessagesA = string.Empty;
        TopicConsumer.ReceivedMessagesB = string.Empty;
    }
}
