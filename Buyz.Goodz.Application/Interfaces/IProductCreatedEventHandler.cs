using Buyz.Goodz.Domain.Events;

namespace Buyz.Goodz.Application.Interfaces;

public interface IProductCreatedEventHandler
{
    Task Handle(ProductCreatedEvent @event);
}
