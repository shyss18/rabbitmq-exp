using Microsoft.AspNetCore.Mvc;
using RM.Producer.Application.Interfaces;
using RM.Producer.Domain.Enums;

namespace RM.Producer.Api.Endpoints;

public static class MessageEndpoints
{
    public static async Task<IResult> SimplePublishAsync([FromServices] IMessageSender sender, string message)
    {
        await sender.SimplePublishAsync(message);
        
        return Results.Ok();
    }

    public static async Task<IResult> PubSubPublishAsync([FromServices] IMessageSender sender, string message)
    {
        await sender.PubSubPublishAsync(message);
        
        return Results.Ok();
    }
    
    public static async Task<IResult> RoutingPublishAsync([FromServices] IMessageSender sender, string message, DirectRoutingKeys? routingKey)
    {
        if (routingKey is null)
        {
            return Results.BadRequest("RoutingKey wasn't specified");
        }
        
        await sender.RoutingPublishAsync(message, routingKey.Value);
        
        return Results.Ok();
    }
    
    public static async Task<IResult> TopicPublishAsync([FromServices] IMessageSender sender, string message, string? topic)
    {
        if (string.IsNullOrWhiteSpace(topic))
        {
            return Results.BadRequest("Topic wasn't specified");
        }
        
        await sender.TopicPublishAsync(message, topic);
        
        return Results.Ok();
    }
}