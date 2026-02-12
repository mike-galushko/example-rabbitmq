using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Example.Options;
using RabbitMQ.Example.Setup;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace RabbitMQ.Example;

public sealed class RpcClient : IAsyncDisposable
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _callbackMapper = new();
    private IConnection? _connection;
    private IChannel? _channel;
    private string? _responseQueueName;

    public RpcClient(QueueOptions options)
    {
        _connectionFactory = options.ToConnectionFactory();
    }

    public async Task StartAsync()
    {
        // Create queue for responses
        // When receiving response trigger ConpletionSource

        // Create queue
        _connection = await _connectionFactory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        var declare = await _channel.QueueDeclareAsync();
        _responseQueueName = declare.QueueName;

        // Subscribe to the queue
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += (model, ea) =>
        {
            // Trigger a callback
            string? correlationId = ea.BasicProperties.CorrelationId;
            if (!string.IsNullOrEmpty(correlationId))
            {
                if (_callbackMapper.TryRemove(correlationId, out var tcs))
                {
                    var body = ea.Body.ToArray();
                    var response = Encoding.UTF8.GetString(body);
                    tcs.TrySetResult(response);
                }
            }
            return Task.CompletedTask;
        };

        await _channel.BasicConsumeAsync(queue: _responseQueueName, autoAck: true, consumer: consumer);
    }

    public async Task<string> CallAsync(string request, CancellationToken cancellationToken = default)
    {
        if (_channel is null)
        {
            throw new InvalidOperationException("Client is not initialized. Please call the StartAsync method first.");
        }

        // Add CorrelationId and ReplayTo propertied into the queue request
        var correlationId = Guid.NewGuid().ToString();
        var props = new BasicProperties
        {
            CorrelationId = correlationId,
            ReplyTo = _responseQueueName,
        };

        // Create task for waiting results
        var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        _callbackMapper.TryAdd(correlationId, tcs);

        // Send request to the queue
        var message = Encoding.UTF8.GetBytes(request);
        await _channel.BasicPublishAsync(
            exchange: string.Empty, 
            routingKey: QueueNames.RpcRequest,
            mandatory: true,
            basicProperties: props,
            body: message);

        // Setup cancellation
        CancellationTokenRegistration ctr = cancellationToken.Register(() =>
        {
            _callbackMapper.TryRemove(correlationId, out _);
            tcs.SetCanceled();
        });

        // Wait task with results
        return await tcs.Task;
    }

    public async ValueTask DisposeAsync()
    {
        // A simple dispose implementation because the calss is marked as 'sealed'
        if (_channel is not null)
        {
            await _channel.CloseAsync();
            _channel = null;
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync();
            _connection = null;
        }
    }
}
