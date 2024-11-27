namespace RM.Producer.Application.Interfaces;

public interface IMessageSender
{
    Task SimplePublishAsync<TMessage>(TMessage message);
}