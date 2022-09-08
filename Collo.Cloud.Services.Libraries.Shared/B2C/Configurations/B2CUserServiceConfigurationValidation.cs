using Microsoft.Extensions.Options;

namespace Collo.Cloud.Services.Libraries.Shared.B2C.Configurations
{
    public class B2cUserServiceConfigurationValidation : IValidateOptions<B2cUserServiceConfiguration>
    {
        public ValidateOptionsResult Validate(string name, B2cUserServiceConfiguration options)
        {
            if (string.IsNullOrEmpty(options.AppId))
                return ValidateOptionsResult.Fail($"{nameof(options.AppId)} configuration parameter for AppId is required");
            if (string.IsNullOrEmpty(options.TenantId))
                return ValidateOptionsResult.Fail($"{nameof(options.TenantId)} configuration parameter for TenantId is required");
            if (string.IsNullOrEmpty(options.ClientSecret))
                return ValidateOptionsResult.Fail($"{nameof(options.ClientSecret)} configuration parameter for ClientSecret is required");
            if (string.IsNullOrEmpty(options.B2cExtensionAppClientId))
                return ValidateOptionsResult.Fail($"{nameof(options.B2cExtensionAppClientId)} configuration parameter for B2cExtensionAppClientId is required");
            if (options.Scopes.Length == 0)
                return ValidateOptionsResult.Fail($"{nameof(options.Scopes)} configuration parameter for Scopes is required");

            return ValidateOptionsResult.Success;
        }
    }
}
