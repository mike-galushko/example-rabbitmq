using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Example.Options;
using RabbitMQ.Example.Setup;
using System.Text;

namespace RabbitMQ.Example;

public class PublishSubscribeProducer
{
    private QueueOptions options;

    public PublishSubscribeProducer(IOptions<QueueOptions> options)
    {
        this.options = options.Value;
    }

    public async Task Send(string message)
    {
        var factory = options.ToConnectionFactory();
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        var bytes = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(exchange: ExchangeNames.Publish, routingKey: string.Empty, body: bytes);
    }
}
