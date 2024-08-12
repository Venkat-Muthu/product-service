using Buyz.Goodz.Application.Interfaces;
using Buyz.Goodz.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Buyz.Goodz.Application.EventHandlers;

public class ProductCreatedEventHandler : IProductCreatedEventHandler
{
    private readonly ILogger<ProductCreatedEventHandler> _logger;
    private readonly INotificationService _notificationService;

    public ProductCreatedEventHandler(ILogger<ProductCreatedEventHandler> logger,
        INotificationService notificationService)
    {
        _logger = logger;
        _notificationService = notificationService;
    }

    public Task Handle(ProductCreatedEvent @event)
    {
        _logger.LogInformation($"Product created: {@event.Name}");
        // Additional handling logic here
        _notificationService.SendProductCreatedNotificationAsync(@event);
        return Task.CompletedTask;
    }
}
