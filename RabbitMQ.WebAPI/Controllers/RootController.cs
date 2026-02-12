using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace RabbitMQ.WebAPI.Controllers;

[EnableCors()]
[Route("api/")]
[ApiController]
public class RootController : ControllerBase
{
    [HttpGet]
    public object Get()
    {
        var info = new 
        {
            Service = "RabbitMQ.WebAPI",
            Revision = "1.1.3",
            Started = DateTime.Now.ToString("yyy-MM-dd HH:mm"),
        };
        return info;
    }
}
