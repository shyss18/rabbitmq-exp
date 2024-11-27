using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RM.Producer.Application.Interfaces;
using RM.Producer.Infrastructure.Options;
using RM.Producer.Infrastructure.Services;

namespace RM.Producer.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));

        services.AddSingleton<IConnectionFactory>(sp =>
        {
            var options = sp.GetRequiredService<RabbitMqOptions>();
            return new ConnectionFactory
            {
                HostName = options.HostName,
                Port = options.Port,
                UserName = options.UserName,
                Password = options.Password,
                VirtualHost = options.VirtualHost
            };
        });

        services.AddSingleton<IMessageSender, MessageSender>();

        return services;
    }
}