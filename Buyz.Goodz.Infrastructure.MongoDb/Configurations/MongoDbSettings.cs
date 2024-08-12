namespace Buyz.Goodz.Infrastructure.MongoDb.Configurations;

public class MongoDbSettings
{
    public static string Name { get; } = nameof(MongoDbSettings);
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public bool UseInMemory { get; set; }
}
