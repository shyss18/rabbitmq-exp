﻿using System.Text;
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

        while (!stoppingToken.IsCancellationRequested)
        {
            var factory = new ConnectionFactory { HostName = _options.HostName };
            await using var connection = await factory.CreateConnectionAsync(stoppingToken);
            await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await channel.QueueDeclareAsync(queue: "hello", durable: false, exclusive: false, autoDelete: false,
                arguments: null, cancellationToken: stoppingToken);
            
            logger.LogInformation(" [*] Waiting for messages.");

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += (_, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                logger.LogInformation($" [x] Received {message}");

                return Task.CompletedTask;
            };

            await channel.BasicConsumeAsync("hello", autoAck: true, consumer: consumer, cancellationToken: stoppingToken);

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }

        logger.LogInformation($"Finish {nameof(SimpleConsumerBackgroundService)}");
    }
}