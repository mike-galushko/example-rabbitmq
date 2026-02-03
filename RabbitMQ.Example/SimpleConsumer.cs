using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMQ.Example;

public sealed class SimpleConsumer : IDisposable
{
    AsyncEventingBasicConsumer? consumer = null;
    IConnection? connection = null;
    IChannel? channel = null;

    // Only one thread writes to that string. No need to add a lock
    // to protect from a concurrent reference update.
    public static string ReceivedMessages = string.Empty;

    public async Task StartListening()
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            Port = 207,
        };
        connection = await factory.CreateConnectionAsync();
        channel = await connection.CreateChannelAsync();
        consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var text = $"[{DateTime.Now:HH:mm:ss}] {message}\n";
            ReceivedMessages = text + ReceivedMessages;
            return Task.CompletedTask;
        };

        await channel.BasicConsumeAsync(queue: QueueNames.Simple, autoAck:true, consumer: consumer);
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
