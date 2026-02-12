using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Example.Setup;
using RabbitMQ.Example.Options;
using System.Text;

namespace RabbitMQ.Example;

public class RpcProducer
{
    private QueueOptions options;

    public RpcProducer(IOptions<QueueOptions> options)
    {
        this.options = options.Value;
    }

    public async Task Send(string message)
    {
        var factory = options.ToConnectionFactory();
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        var bytes = Encoding.UTF8.GetBytes(message);

        var routingKey = GetRoutingKey(message);
        await channel.BasicPublishAsync(exchange: ExchangeNames.Topic, routingKey: routingKey, body: bytes);
    }

    private string GetRoutingKey(string message)
    {
        var parts = message.Split('-');
        return parts.Length == 2 ? parts[0] : message;
    }
}
