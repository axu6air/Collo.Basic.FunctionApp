using Microsoft.Extensions.Options;

namespace Collo.Cloud.Services.Libraries.Shared.Configurations
{
    public class EventGridServiceConfigurationValidation : IValidateOptions<EventGridServiceConfiguration>
    {
        public ValidateOptionsResult Validate(string name, EventGridServiceConfiguration options)
        {
            if (string.IsNullOrEmpty(options.AzureEventGridTopicEndpoint))
                throw new ArgumentNullException(nameof(options.AzureEventGridTopicEndpoint));

            if (string.IsNullOrEmpty(options.AzureEventGridTopicAccessKey))
                throw new ArgumentNullException(nameof(options.AzureEventGridTopicAccessKey));

            return ValidateOptionsResult.Success;
        }
    }
}
