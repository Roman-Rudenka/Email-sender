using System.Text;
using Application.Interfaces;
using Application.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;


namespace Application.Services;

public class RabbitMqListener : BackgroundService
{
    private readonly IQueryService _queryService;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly RabbitOptions _options;
    public RabbitMqListener(IQueryService queryService,  IOptions<RabbitOptions> options)
    {
        _queryService = queryService;
        _options = options.Value;
        
        var factory = new ConnectionFactory
        {
            Uri = new Uri(_options.ConnectionUri)
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: _options.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (sender, ea) =>
        {
            var body = ea.Body.ToArray();
            var email = Encoding.UTF8.GetString(body);

            try
            {
                await _queryService.SendEmailMessageAsync(email, "info", "Data successfully added");
                Console.WriteLine($"[✓] Email sent to: {email}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Error sending email: {ex.Message}");
            }

            _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };

        _channel.BasicConsume(queue: _options.QueueName,
            autoAck: false,
            consumer: consumer);

        Console.WriteLine("[*] RabbitMQ listener started");

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}
