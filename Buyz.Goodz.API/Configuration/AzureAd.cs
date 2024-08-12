namespace Buyz.Goodz.API.Configuration;

public class AzureAd
{
    public string Instance { get; set; }
    public string TenantId { get; set; }
    public string Domain { get; set; }
    public string ClientId { get; set; }
    public string SwaggerUiGatewayClientId { get; set; }
    public string SwaggerUiGatewaySecret { get; set; }
    public string SwaggerOpenIdSignInCallBack { get; set; }
}