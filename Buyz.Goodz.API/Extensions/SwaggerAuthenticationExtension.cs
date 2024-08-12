using Buyz.Goodz.API.Configuration;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

namespace Buyz.Goodz.API.Extensions;

public static class SwaggerAuthenticationExtension
{
    public static IServiceCollection AddSwaggerAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        AzureAd azureAd = new AzureAd();
        configuration.GetSection(nameof(AzureAd)).Bind(azureAd);

        services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(identity =>
            {
                identity.CallbackPath = azureAd.SwaggerOpenIdSignInCallBack;
                identity.ClientId = azureAd.SwaggerUiGatewayClientId;
                identity.ClientSecret = azureAd.SwaggerUiGatewaySecret;
                identity.TenantId = azureAd.TenantId;
                identity.Instance = azureAd.Instance;

            });

        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromHours(1);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo { Title = "Products API", Version = "v1" });
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{}
                }
            });
        });

        return services;
    }
}