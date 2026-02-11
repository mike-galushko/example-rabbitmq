using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Example.Setup;

namespace RabbitMQ.WebAPI.Controllers;

[EnableCors()]
[Route("api/[controller]")]
[ApiController]
public class ResetController : ControllerBase
{
    [HttpPost]
    public void PostAsync()
    {
        QueueInitialization.Reset();
    }
}
