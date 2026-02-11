using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Example.Setup;
using RabbitMQ.Example.Options;
using System.Text;

namespace RabbitMQ.Example;

public class RoutingProducer
{
    private QueueOptions options;

    public RoutingProducer(IOptions<QueueOptions> options)
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
        await channel.BasicPublishAsync(exchange: ExchangeNames.Routing, routingKey: routingKey, body: bytes);
    }

    private string GetRoutingKey(string message)
    {
        if (message.StartsWith("a", StringComparison.OrdinalIgnoreCase))
            return RoutingKeys.RoutingA;
        if (message.StartsWith("b", StringComparison.OrdinalIgnoreCase))
            return RoutingKeys.RoutingB;

        return string.Empty;
    }
}
