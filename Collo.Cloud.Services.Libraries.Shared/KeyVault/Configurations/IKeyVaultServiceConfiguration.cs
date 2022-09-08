namespace Collo.Cloud.Services.Libraries.Shared.KeyVault.Configurations
{
    public interface IKeyVaultServiceConfiguration
    {
        string Url { get; set; }
        string TenantId { get; set; }
        string AppId { get; set; }
        string ClientSecret { get; set; }
    }
}
