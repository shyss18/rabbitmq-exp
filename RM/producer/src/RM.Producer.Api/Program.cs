using Microsoft.AspNetCore.Mvc;
using RM.Producer.Api.Endpoints;
using RM.Producer.Api.Models;
using RM.Producer.Application.Interfaces;
using RM.Producer.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("simple",
    async ([FromServices] IMessageSender sender, [FromBody] MessageModel messageModel) =>
    await MessageEndpoints.SimplePublishAsync(sender, messageModel.Message));

app.MapPost("pub-sub",
    async ([FromServices] IMessageSender sender, [FromBody] MessageModel messageModel) =>
    await MessageEndpoints.PubSubPublishAsync(sender, messageModel.Message));

app.MapPost("routing",
    async ([FromServices] IMessageSender sender, [FromBody] MessageModel messageModel) =>
    await MessageEndpoints.RoutingPublishAsync(sender, messageModel.Message, messageModel.RoutingKey));

app.MapPost("topic",
    async ([FromServices] IMessageSender sender, [FromBody] MessageModel messageModel) =>
    await MessageEndpoints.TopicPublishAsync(sender, messageModel.Message, messageModel.Topic));

app.Run();