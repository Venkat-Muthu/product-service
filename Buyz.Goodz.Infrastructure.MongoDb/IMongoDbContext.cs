using MongoDB.Driver;

namespace Buyz.Goodz.Infrastructure.MongoDb;

public interface IMongoDbContext
{
    IMongoCollection<T> GetCollection<T>(string name);
}
