using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RM.Producer.Application.Interfaces;

namespace RM.Producer.Infrastructure.Services;

internal class MessageSender : IMessageSender
{
    private readonly IConnectionFactory _connectionFactory;

    public MessageSender(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task SimplePublishAsync<TMessage>(TMessage message)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        var serializedMessage = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(serializedMessage);

        await channel.BasicPublishAsync(string.Empty, routingKey: "hello", body);
    }
}