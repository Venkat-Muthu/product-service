using Buyz.Goodz.Application.Commands;
using Buyz.Goodz.Application.EventHandlers;
using Buyz.Goodz.Application.Interfaces;
using Buyz.Goodz.Application.Mappers;
using Buyz.Goodz.Application.Queries;
using Buyz.Goodz.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Buyz.Goodz.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register Application Services
        services.AddScoped<IProductDtoMapper, ProductDtoMapper>();

        // Register other application services, command/query handlers, etc.
        services.AddScoped<IProductCommandService, ProductCommandService>();
        services.AddScoped<IProductQueryService, ProductQueryService>();
        services.AddScoped<IProductCreatedEventHandler, ProductCreatedEventHandler>();
        services.AddScoped<INotificationService, MessageBrokerNotificationService>();

        return services;
    }
}