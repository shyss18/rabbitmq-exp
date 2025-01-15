namespace RM.Producer.Api.Models;

public record MessageModel
{
    public required string Message { get; set; }

    public string RoutingKey { get; set; } = string.Empty;
}