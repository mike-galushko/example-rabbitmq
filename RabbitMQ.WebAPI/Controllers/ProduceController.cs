using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Example;
using RabbitMQ.WebAPI.Models;

namespace RabbitMQ.WebAPI.Controllers;

[EnableCors()]
[Route("api/[controller]")]
[ApiController]
public class ProduceController : ControllerBase
{
    [HttpPost]
    public async Task<PostResponse?> PostAsync([FromBody]PostRequest model)
    {
        PostResponse? response = null;

        if (model.Type == "simple")
        {
            await new SimpleProducer().Send(model.Message);
            response = new PostResponse
            {
                ConsumerA = SimpleConsumer.ReceivedMessages,
                ConsumerB = "",
            };
        }
        else if (model.Type == "worker")
        {
            // Send message multiple times, so to make worker tasks busy
            for (int i = 0; i < 6; i++)
            {
                await new WorkerQueueProducer().Send($"{model.Message} #{i+1}");
            }

            // Give a time to worker tasks to complete
            await Task.Delay(1000);
            
            response = new PostResponse
            {
                ConsumerA = WorkerQueueConsumer.ReceivedMessagesA,
                ConsumerB = WorkerQueueConsumer.ReceivedMessagesB,
            };
        }

        return response;
    }
}
