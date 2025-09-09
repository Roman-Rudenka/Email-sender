using System.Text;
using Application.Interfaces;
using Application.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Application.Services;

public class RabbitMqService(IOptions<RabbitOptions> rabbitOptions) : IRabbitMqService
{
    private readonly RabbitOptions _rabbitOptions = rabbitOptions.Value;

    public void SendEmail(string email)
    {
        var factory = new ConnectionFactory()
        {
            Uri = new Uri(_rabbitOptions.ConnectionUri) 
        };

        using var connection =  factory.CreateConnection();
        using var channel = connection.CreateModel();

        Console.WriteLine("queue" + _rabbitOptions.QueueName);
        channel.QueueDeclare(queue: _rabbitOptions.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var body = Encoding.UTF8.GetBytes(email);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;

        channel.BasicPublish(exchange: "",
            routingKey: _rabbitOptions.QueueName,
            basicProperties: properties,
            body: body);

        Console.WriteLine($"[x] Email address sent: {email}");
    }
}