using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Example.Setup;
using RabbitMQ.Example.Options;
using System.Text;

namespace RabbitMQ.Example;

public sealed class PublishSubscribeConsumer : IDisposable
{
    AsyncEventingBasicConsumer? consumer = null;
    IConnection? connection = null;
    IChannel? channel = null;

    // Only one thread writes to that string. No need to add a lock
    // to protect from a concurrent reference update.
    public static string ReceivedMessagesA = string.Empty;
    public static string ReceivedMessagesB = string.Empty;

    private int consumerId = 0;
    private QueueOptions options;

    public PublishSubscribeConsumer(QueueOptions options, int consumerId)
    {
        this.consumerId = consumerId;
        this.options = options;
    }

    public async Task StartListening()
    {
        var factory = options.ToConnectionFactory();

        connection = await factory.CreateConnectionAsync();
        channel = await connection.CreateChannelAsync();

        consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var text = $"[{DateTime.Now:HH:mm:ss}] {message}\n";
            if (consumerId == 1)
            {
                ReceivedMessagesA = text + ReceivedMessagesA;
            }
            else 
            {
                ReceivedMessagesB = text + ReceivedMessagesB;
            }
        };

        var queue = consumerId == 1 ? QueueNames.PublishA : QueueNames.PublishB;
        await channel.BasicConsumeAsync(queue: queue, autoAck:true, consumer: consumer);
    }

    public void Dispose()
    {
        // A simple dispose implementation because the calss is marked as 'sealed'
        if (connection != null)
        {
            connection.Dispose();
            connection = null;
        }

        if (channel != null)
        {
            channel.Dispose();
            channel = null;
        }
    }
}
