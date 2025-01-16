using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RM.Consumer.Domain.Constants;
using RM.Consumer.Infrastructure.Options;

namespace RM.Consumer.Infrastructure.Consumers;

internal sealed class TopicConsumerBackgroundService(
    ILogger<TopicConsumerBackgroundService> logger,
    IOptions<RabbitMqOptions> options)
    : BackgroundService
{
    private readonly RabbitMqOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"Start {nameof(TopicConsumerBackgroundService)}");

        logger.LogInformation(" [*] Waiting for messages.");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var factory = new ConnectionFactory { HostName = _options.HostName };
            await using var connection = await factory.CreateConnectionAsync(stoppingToken);
            await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await channel.ExchangeDeclareAsync(exchange: "topic_logs", type: ExchangeType.Topic, cancellationToken: stoppingToken);

            var queueDeclareResult = await channel.QueueDeclareAsync(cancellationToken: stoppingToken);
            var queueName = queueDeclareResult.QueueName;

            foreach (var bindingKey in Topics.Values)
            {
                await channel.QueueBindAsync(queue: queueName, exchange: "topic_logs", routingKey: bindingKey, cancellationToken: stoppingToken);
            }
            
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += (_, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;

                logger.LogInformation($" [x] Received '{routingKey}':'{message}'");

                return Task.CompletedTask;
            };

            await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer, cancellationToken: stoppingToken);

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }

        logger.LogInformation($"Finish {nameof(TopicConsumerBackgroundService)}");
    }
}