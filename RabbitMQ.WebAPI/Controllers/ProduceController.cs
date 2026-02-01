using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Example;
using RabbitMQ.WebAPI.Models;

namespace RabbitMQ.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProduceController : ControllerBase
{
    [HttpPost]
    public PostResponse? Post([FromBody]PostRequest model)
    {
        PostResponse? response = null;
        if (model.Type == "simple")
        {
            new SimpleProducer().Send(model.Message);
            response = new PostResponse
            {
                ConsumerA = string.Join("\n", SimpleProducer.Messages),
                ConsumerB = "",
            };
        }
        return response;
    }
}
