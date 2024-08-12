using Buyz.Goodz.Application.Interfaces;
using Buyz.Goodz.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Buyz.Goodz.Application.Services;

public class MessageBrokerNotificationService : INotificationService
{
    private readonly ILogger<MessageBrokerNotificationService> _logger;

    public MessageBrokerNotificationService(ILogger<MessageBrokerNotificationService> logger)
    {
        _logger = logger;
    }

    public async Task SendProductCreatedNotificationAsync(ProductCreatedEvent @event,
        CancellationToken cancellationToken = default)
    {
        //TODO: Use Azure Service Bus Topic or RabbitMQ or some other messaging service to publish the event.
        _logger.LogInformation("Published ProductCreatedEvent for ProductId: {ProductId}", @event.ProductId);
        await Task.FromResult(0);
    }
}