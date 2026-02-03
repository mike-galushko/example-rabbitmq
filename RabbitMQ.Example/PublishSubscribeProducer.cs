using RabbitMQ.Client;
using System.Text;

namespace RabbitMQ.Example;

public class PublishSubscribeProducer
{
    public async Task Send(string message)
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            Port = 207,
        };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        var bytes = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(exchange: ExchangeNames.Publish, routingKey: string.Empty, body: bytes);
    }
}
