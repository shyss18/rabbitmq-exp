using RM.Producer.Domain.Enums;

namespace RM.Producer.Api.Models;

public record MessageModel
{
    public required string Message { get; set; }

    public DirectRoutingKeys? RoutingKey { get; set; }

    public string? Topic { get; set; }
}