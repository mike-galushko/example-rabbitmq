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
        return response;
    }
}
