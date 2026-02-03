using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Example;

namespace RabbitMQ.WebAPI.Controllers;

[EnableCors()]
[Route("api/")]
[ApiController]
public class RootController : ControllerBase
{
    [HttpGet]
    public string Get()
    {
        return "RabbitMQ.WebAPI v1.0";
    }
}
