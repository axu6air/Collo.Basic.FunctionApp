namespace Collo.Cloud.Services.Libraries.Shared.B2C.Configurations
{
    public interface IB2cUserServiceConfiguration
    {
        string TenantId { get; set; }
        string AppId { get; set; }
        string Issuer { get; set; }
        string ClientSecret { get; set; }
        string B2cExtensionAppClientId { get; set; }
        public string[] Scopes { get; set; }
    }
}
