using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RabbitMQ.Example;
using RabbitMQ.Example.Options;
using RabbitMQ.WebAPI.Models;

namespace RabbitMQ.WebAPI.Controllers;

[EnableCors()]
[Route("api/[controller]")]
[ApiController]
public class ProduceController : ControllerBase
{
    private readonly SimpleProducer simple;
    private readonly WorkerQueueProducer worker;
    private readonly PublishSubscribeProducer publish;
    private readonly RoutingProducer routing;
    private readonly TopicProducer topic;
    private readonly QueueOptions options;

    public ProduceController(IOptions<QueueOptions> options, SimpleProducer simple, WorkerQueueProducer worker, PublishSubscribeProducer publish,
        RoutingProducer routing, TopicProducer topic)
    {
        this.simple = simple;
        this.worker = worker;
        this.publish = publish;
        this.routing = routing;
        this.topic = topic;
        this.options = options.Value;
    }

    [HttpPost]
    public async Task<PostResponse?> PostAsync([FromBody]PostRequest model)
    {
        PostResponse? response = null;

        var type = model.Type.ToLower();
        if (type == "simple")
        {
            await simple.Send(model.Message);
            response = new PostResponse
            {
                ConsumerA = SimpleConsumer.ReceivedMessages,
                ConsumerB = "",
            };
        }
        else if (type == "worker")
        {
            // Send message multiple times, so to make worker tasks busy
            for (int i = 0; i < 6; i++)
            {
                await worker.Send($"{model.Message} #{i+1}");
            }

            // Give a time to worker tasks to complete
            await Task.Delay(1000);

            response = new PostResponse
            {
                ConsumerA = WorkerQueueConsumer.ReceivedMessagesA,
                ConsumerB = WorkerQueueConsumer.ReceivedMessagesB,
            };
        }
        else if (type == "publish")
        {
            await publish.Send(model.Message);
            response = new PostResponse
            {
                ConsumerA = PublishSubscribeConsumer.ReceivedMessagesA,
                ConsumerB = PublishSubscribeConsumer.ReceivedMessagesB,
            };
        }
        else if (type == "routing")
        {
            await routing.Send(model.Message);
            response = new PostResponse
            {
                ConsumerA = RoutingConsumer.ReceivedMessagesA,
                ConsumerB = RoutingConsumer.ReceivedMessagesB,
            };
        }
        else if (type == "topic")
        {
            await topic.Send(model.Message);
            response = new PostResponse
            {
                ConsumerA = TopicConsumer.ReceivedMessagesA,
                ConsumerB = TopicConsumer.ReceivedMessagesB,
            };
        }
        else if (type == "rpc")
        {
            await using var client = new RpcClient(options);
            await client.StartAsync();
            var result = await client.CallAsync(model.Message);
            response = new PostResponse
            {
                ConsumerA = result,
                ConsumerB = string.Empty, 
            };
        }
        else
        {
            throw new Exception("Unknown routing type specified");
        }

            return response;
    }
}
