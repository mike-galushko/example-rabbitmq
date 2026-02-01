namespace RabbitMQ.WebAPI.Models;

public class PostRequest
{
    public required string Type { get; set; }
    public required string Message { get; set; }
}
