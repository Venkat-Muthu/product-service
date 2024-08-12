using AutoFixture;
using AutoFixture.Xunit2;
using Buyz.Goodz.Application.EventHandlers;
using Buyz.Goodz.Application.Interfaces;
using Buyz.Goodz.Domain.Events;
using Buyz.Goodz.TestUtils.Mocks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Buyz.Goodz.Application.UnitTests.EventHandlers;

public class ProductCreatedEventHandlerTests
{
    private readonly IFixture _fixture;
    private readonly MockLogger<ProductCreatedEventHandler> _logger;
    private readonly INotificationService _notificationService;

    public ProductCreatedEventHandlerTests()
    {
        _fixture = new Fixture();
        _logger = Substitute.For<MockLogger<ProductCreatedEventHandler>>();
        _notificationService = Substitute.For<INotificationService>();
    }

    [Theory, AutoData]
    public async Task Handle_ShouldLogInformationAndSendNotification(
        ProductCreatedEvent @event)
    {
        // Arrange
        var eventHandler = new ProductCreatedEventHandler(_logger, _notificationService);

        // Act
        await eventHandler.Handle(@event);

        // Assert
        // Verify that the logger's LogInformation method was called with the correct message
        _logger.Received(1).LogInformation($"Product created: {@event.Name}");

        // Verify that the notification service's SendProductCreatedNotificationAsync method was called
        await _notificationService.Received(1).SendProductCreatedNotificationAsync(@event);
    }
}
