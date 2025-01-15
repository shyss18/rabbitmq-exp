using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RM.Producer.Application.Interfaces;

namespace RM.Producer.Infrastructure.Services;

internal class MessageSender(IConnectionFactory connectionFactory) : IMessageSender
{
    public async Task SimplePublishAsync<TMessage>(TMessage message)
    {
        await using var connection = await connectionFactory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        var serializedMessage = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(serializedMessage);

        await channel.QueueDeclareAsync(queue: "task_queue", durable: true, exclusive: false, autoDelete: false,
            arguments: null);

        var properties = new BasicProperties
        {
            Persistent = true
        };
        
        await channel.BasicPublishAsync(string.Empty, routingKey: "task_queue", mandatory: true, basicProperties: properties, body);
    }

    public async Task PubSubPublishAsync<TMessage>(TMessage message)
    {
        await using var connection = await connectionFactory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        var serializedMessage = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(serializedMessage);

        await channel.ExchangeDeclareAsync(exchange: "logs", type: ExchangeType.Fanout);
        await channel.BasicPublishAsync("logs", routingKey: string.Empty, body);
    }
}