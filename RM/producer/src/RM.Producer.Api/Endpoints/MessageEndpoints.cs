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
}