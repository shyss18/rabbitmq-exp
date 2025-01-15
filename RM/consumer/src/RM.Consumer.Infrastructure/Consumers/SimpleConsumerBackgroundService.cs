using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RM.Consumer.Infrastructure.Options;

namespace RM.Consumer.Infrastructure.Consumers;

internal sealed class SimpleConsumerBackgroundService(
    ILogger<SimpleConsumerBackgroundService> logger,
    IOptions<RabbitMqOptions> options)
    : BackgroundService
{
    private readonly RabbitMqOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"Start {nameof(SimpleConsumerBackgroundService)}");

        logger.LogInformation(" [*] Waiting for messages.");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var factory = new ConnectionFactory { HostName = _options.HostName };
            await using var connection = await factory.CreateConnectionAsync(stoppingToken);
            await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await channel.QueueDeclareAsync(queue: "task_queue", durable: true, exclusive: false, autoDelete: false,
                arguments: null, cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (_, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                logger.LogInformation($" [x] Received {message}");

                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, false, stoppingToken);
            };

            await channel.BasicConsumeAsync("task_queue", autoAck: false, consumer: consumer, cancellationToken: stoppingToken);

            await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: stoppingToken);
            
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }

        logger.LogInformation($"Finish {nameof(SimpleConsumerBackgroundService)}");
    }
}