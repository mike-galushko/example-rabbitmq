using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Example.Setup;
using RabbitMQ.Example.Options;
using System.Text;

namespace RabbitMQ.Example;

public sealed class RpcConsumer: IDisposable
{
    AsyncEventingBasicConsumer? consumer = null;
    IConnection? connection = null;
    IChannel? channel = null;

    private QueueOptions options;

    public RpcConsumer(QueueOptions options)
    {
        this.options = options;
    }

    public async Task StartListening()
    {
        var factory = options.ToConnectionFactory();

        connection = await factory.CreateConnectionAsync();
        channel = await connection.CreateChannelAsync();

        consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (object sender, BasicDeliverEventArgs ea) =>
        {
            var body = ea.Body.ToArray();
            var props = ea.BasicProperties;
            var replayProps = new BasicProperties { CorrelationId = props.CorrelationId };

            var response = string.Empty;
            try
            {
                var message = Encoding.UTF8.GetString(body);
                response = $"[{DateTime.Now:HH:mm:ss}] {message}\n";
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }
            finally
            {
                var consuer = (AsyncEventingBasicConsumer)sender;
                var channel = consuer.Channel;

                var responseBytes = Encoding.UTF8.GetBytes(response);
                await channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: props.ReplyTo!,
                    mandatory: true,
                    basicProperties: replayProps,
                    body: responseBytes);

                await channel.BasicAckAsync(
                    deliveryTag: ea.DeliveryTag, 
                    multiple: false);
            }
        };

        await channel.BasicConsumeAsync(queue: QueueNames.RpcRequest, autoAck:false, consumer: consumer);
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
