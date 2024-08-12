using AutoFixture;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;
using Buyz.Goodz.Application.Interfaces;
using Buyz.Goodz.Application.Services;
using Buyz.Goodz.Domain.Events;
using Buyz.Goodz.TestUtils.Mocks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Buyz.Goodz.Application.UnitTests.Services;

public class MessageBrokerNotificationServiceTests
{
    IFixture _fixture = new Fixture();
    private readonly MockLogger<MessageBrokerNotificationService> _logger;
    private readonly INotificationService _notificationService;
    public MessageBrokerNotificationServiceTests()
    {
        _fixture.Customizations.Add(
            new TypeRelay(
                typeof(ILogger<MessageBrokerNotificationServiceTests>),
                typeof(MessageBrokerNotificationServiceTests)));

        _logger = Substitute.For<MockLogger<MessageBrokerNotificationService>>();
        _notificationService = new MessageBrokerNotificationService(_logger);
    }

    [Theory, AutoData]
    public async Task SendProductCreatedNotificationAsync_ShouldLogInformation(
         ProductCreatedEvent @event)
    {
        // Act
        await _notificationService.SendProductCreatedNotificationAsync(@event);

        // Assert
        _logger.Received(1).Log(
            LogLevel.Information,
            Arg.Is<string>(o => o.ToString().Contains($"Published ProductCreatedEvent for ProductId: {@event.ProductId}"))
            );
    }

    [Theory, AutoData]
    public async Task SendProductCreatedNotificationAsync_ShouldCompleteTask(
          ProductCreatedEvent @event)
    {
        // Act
        Func<Task> action = () => _notificationService.SendProductCreatedNotificationAsync(@event);

        // Assert
        await action.Should().NotThrowAsync();
    }
}