using Buyz.Goodz.API.Extensions;
using Buyz.Goodz.Infrastructure.MongoDb.DependencyInjection;
using Buyz.Goodz.Application.DependencyInjection;

namespace Buyz.Goodz.API;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddHealthCheck();
        builder.Services.AddSwaggerAuthentication(builder.Configuration);

        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddApplicationServices();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseMiddleware<SwaggerOAuthMiddleware>(app.Configuration);
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
