using Mongo2Go;

namespace Buyz.Goodz.API.Extensions;

public static class MongoDbRunnerExtension
{
    public static void UseMongoDbRunner(this WebApplication app)
    {
        var mongoRunner = app.Services.GetRequiredService<MongoDbRunner>();

        var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
        lifetime.ApplicationStopping.Register(mongoRunner.Dispose);
    }
}