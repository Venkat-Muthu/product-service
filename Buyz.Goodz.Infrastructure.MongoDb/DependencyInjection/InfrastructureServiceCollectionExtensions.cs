using Buyz.Goodz.Domain.Repositories;
using Buyz.Goodz.Infrastructure.MongoDb.Configurations;
using Buyz.Goodz.Infrastructure.MongoDb.Documents;
using Buyz.Goodz.Infrastructure.MongoDb.Mappers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mongo2Go;
using MongoDB.Driver;
using System.Text.Json;

namespace Buyz.Goodz.Infrastructure.MongoDb.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    private static IServiceCollection AddInMemoryDatabase(this IServiceCollection services,
         IConfiguration configuration)
    {
        //Read Configs
        var settings = configuration.GetSection(MongoDbSettings.Name).Get<MongoDbSettings>();

        // Register the DbContext
        var _runner = MongoDbRunner.Start();
        var client = new MongoClient(_runner.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);

        database.CreateCollection(settings.DatabaseName);
        IMongoCollection<ProductDocument> _products = database.GetCollection<ProductDocument>("Products");
        LoadProductsFromJson(_products, "./Seeds/products.json");

        services.AddSingleton(_runner);
        services.AddSingleton<IMongoDatabase>(database);

        return services;
    }
    private static IServiceCollection AddDatabase(this IServiceCollection services,
         IConfiguration configuration)
    {
        //Read Configs
        var settings = configuration.GetSection(MongoDbSettings.Name).Get<MongoDbSettings>();

        // Register the DbContext
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);
        services.AddSingleton<IMongoDatabase>(database);

        return services;
    }
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        //Read Configs
        var settings = configuration.GetSection(MongoDbSettings.Name).Get<MongoDbSettings>();
        if (!string.IsNullOrWhiteSpace(settings.ConnectionString) && settings.UseInMemory)
        {
            throw new Exception("MongoDbSettings has conflicting settings. Please remove either ConnectionString or UseInMemory");
        }

        if (settings.UseInMemory)
        {
            services.AddInMemoryDatabase(configuration);
        }
        else
        {
            services.AddDatabase(configuration);
        }

        // Register Repositories
        services.AddScoped<IMongoDbContext, MongoDbContext>();
        services.AddScoped<IProductReadRepository, ProductRepository>();
        services.AddScoped<IProductWriteRepository, ProductRepository>();

        // Register other infrastructure services or adapters
        services.AddScoped<IProductDocumentMapper, ProductDocumentMapper>();

        return services;
    }

    private static void LoadProductsFromJson(IMongoCollection<ProductDocument> mongoDb,
        string filePath)
    {
        if (File.Exists(filePath))
        {
            var jsonData = File.ReadAllText(filePath);
            var products = JsonSerializer.Deserialize<List<ProductDocument>>(jsonData);
            if (products != null)
            {
                foreach (var product in products)
                {
                    mongoDb.InsertOne(product);
                }
            }
        }
    }
}