using Microsoft.AspNetCore.Mvc;
using RM.Producer.Application.Interfaces;

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
    
    public static async Task<IResult> RoutingPublishAsync([FromServices] IMessageSender sender, string message, string routingKey)
    {
        await sender.RoutingPublishAsync(message, routingKey);
        
        return Results.Ok();
    }
}