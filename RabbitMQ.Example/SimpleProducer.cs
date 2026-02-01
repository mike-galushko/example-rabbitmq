namespace RabbitMQ.Example;

public class SimpleProducer
{
    public static List<string> Messages = new();

    public void Send(string message)
    {
        Messages.Add(message);
    }
}
