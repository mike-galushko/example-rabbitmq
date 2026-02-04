using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Example;

namespace RabbitMQ.WebAPI.Controllers;

[EnableCors()]
[Route("api/[controller]")]
[ApiController]
public class ResetController : ControllerBase
{
    [HttpPost]
    public void PostAsync()
    {
        SimpleConsumer.ReceivedMessages = string.Empty;
        WorkerQueueConsumer.ReceivedMessagesA = string.Empty;
        WorkerQueueConsumer.ReceivedMessagesB = string.Empty;
        PublishSubscribeConsumer.ReceivedMessagesA = string.Empty;
        PublishSubscribeConsumer.ReceivedMessagesB = string.Empty;
        RoutingConsumer.ReceivedMessagesA = string.Empty;
        RoutingConsumer.ReceivedMessagesB = string.Empty;
    }
}
