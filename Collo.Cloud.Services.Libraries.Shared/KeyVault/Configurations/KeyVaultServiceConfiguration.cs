namespace Collo.Cloud.Services.Libraries.Shared.KeyVault.Configurations
{
    public class KeyVaultServiceConfiguration : IKeyVaultServiceConfiguration
    {
        public string Url { get; set; }
        public string TenantId { get; set; }
        public string AppId { get; set; }
        public string ClientSecret { get; set; }
    }
}
