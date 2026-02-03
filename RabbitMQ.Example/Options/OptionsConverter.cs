using RabbitMQ.Client;

namespace RabbitMQ.Example.Options;

internal static class OptionsConverter
{
    public static ConnectionFactory ToConnectionFactory(this QueueOptions options)
    {
        var connection = new ConnectionFactory
        {
            HostName = options.Host
        };

        if (options.Port != null)
        {
            connection.Port = options.Port.Value;
        }

        return connection;
    }
}
