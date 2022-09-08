using Microsoft.Extensions.Options;

namespace Collo.Cloud.Templates.Shared.Configurations
{
    public class StorageServiceConfigurationValidation : IValidateOptions<StorageServiceConfiguration>
    {
        public ValidateOptionsResult Validate(string name, StorageServiceConfiguration options)
        {
            if (string.IsNullOrEmpty(options.ConnectionString))
                return ValidateOptionsResult.Fail($"{nameof(options.ConnectionString)} configuration parameter for the Azure Storage Account is required");

            if (string.IsNullOrEmpty(options.ContainerName))
                return ValidateOptionsResult.Fail($"{nameof(options.ContainerName)} configuration parameter for the Azure Storage Account is required");

            return ValidateOptionsResult.Success;
        }
    }
}
