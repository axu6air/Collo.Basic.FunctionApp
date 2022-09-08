using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Collo.Cloud.Services.Libraries.Shared.KeyVault.Configurations;
using Microsoft.Extensions.Configuration;

namespace Collo.Cloud.Services.Libraries.Shared.KeyVault
{
    public static class KeyVaultServiceExtension
    {
        /// <summary>
        /// Used for injecting Azure Key Vault service
        /// </summary>
        /// <param name="key">is Environment Variable Name</param>
        /// <returns></returns>
        public static void AddKeyVaultService(this IConfigurationBuilder configuration, string key)
        {
            //Get Value from Environment
            var keyVaultConfig = Helpers.EnvironmentVariable<KeyVaultServiceConfiguration>.GetServiceConfiguration(key);

            var credential = new ClientSecretCredential(keyVaultConfig.TenantId, keyVaultConfig.AppId, keyVaultConfig.ClientSecret);
            var client = new SecretClient(new Uri(keyVaultConfig.Url), credential);
            configuration.AddAzureKeyVault(client, new AzureKeyVaultConfigurationOptions());
        }
    }
}
