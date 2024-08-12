using Buyz.Goodz.Domain.Events;

namespace Buyz.Goodz.Application.Interfaces;

public interface INotificationService
{
    Task SendProductCreatedNotificationAsync(ProductCreatedEvent @event,
        CancellationToken cancellationToken = default);
}
