using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;


namespace Buyz.Goodz.API;

public class SwaggerOAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private OpenIdConnectConfiguration _openIdConnectConfiguration;

    public SwaggerOAuthMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (IsSwaggerUi(context.Request.Path) && !await IsValidIdToken(GetIdTokenFromSessionStorage(context)))
        {
            await context.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme);
            return;
        }

        if (IsSwaggerOpenIdSignInCallBack(context.Request.Path))
        {
            var idToken = await GetIdTokenFromSignInCallBackRequest(context);
            if (!await IsValidIdToken(idToken))
            {
                await context.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme);
                return;
            }
            context.Session.SetString("id_token", idToken);
        }

        await _next.Invoke(context).ConfigureAwait(false);
    }

    private static bool IsSwaggerUi(PathString pathString)
    {
        return pathString.StartsWithSegments("/swagger");
    }

    private bool IsSwaggerOpenIdSignInCallBack(PathString pathString)
    {
        var swaggerOpenIdSignInCallBack = _configuration.GetValue<string>("AzureAd:SwaggerOpenIdSignInCallBack");
        return pathString.StartsWithSegments(swaggerOpenIdSignInCallBack);
    }

    private static string GetIdTokenFromSessionStorage(HttpContext context)
    {
        return context.Session.GetString("id_token");
    }

    private static async Task<string> GetIdTokenFromSignInCallBackRequest(HttpContext context)
    {
        context.Request.EnableBuffering();
        var reader = new StreamReader(context.Request.Body);
        var body = await reader.ReadToEndAsync();
        context.Request.Body.Seek(0, SeekOrigin.Begin);

        return body
        .Split("&")
        .FirstOrDefault(e => e.Contains("id_token"))?
            .Split("=")
        .LastOrDefault();
    }

    private async Task<bool> IsValidIdToken(string idToken)
    {
        if (string.IsNullOrEmpty(idToken))
            return false;

        await LoadOpenIdConnectConfiguration();
        var instance = _configuration.GetValue<string>("AzureAd:Instance");
        var tenantId = _configuration.GetValue<string>("AzureAd:TenantId");
        var swaggerUiGatewayClientId = _configuration.GetValue<string>("AzureAd:SwaggerUiGatewayClientId");
        var jwtHandler = new JwtSecurityTokenHandler();
        var result = await jwtHandler.ValidateTokenAsync(idToken,
        new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = swaggerUiGatewayClientId,
            ValidateIssuer = true,
            ValidIssuer = $"{instance}{tenantId}/v2.0",
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = _openIdConnectConfiguration.SigningKeys,
            RequireExpirationTime = true,
            ValidateLifetime = true,
            RequireSignedTokens = true
        });
        return result.IsValid;
    }

    private async Task LoadOpenIdConnectConfiguration()
    {
        if (_openIdConnectConfiguration is null)
        {
            var instance = _configuration.GetValue<string>("AzureAd:Instance");
            var tenantId = _configuration.GetValue<string>("AzureAd:TenantId");

            var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            $"{instance}{tenantId}/v2.0/.well-known/openid-configuration",
            new OpenIdConnectConfigurationRetriever());

            _openIdConnectConfiguration = await configManager.GetConfigurationAsync();
        }
    }
}