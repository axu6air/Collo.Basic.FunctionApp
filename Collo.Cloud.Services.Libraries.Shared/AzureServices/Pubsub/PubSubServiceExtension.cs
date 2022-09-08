using Collo.Cloud.Services.Libraries.Shared.AzureServices.Pubsub;
using Microsoft.Extensions.DependencyInjection;

namespace Collo.Cloud.Services.Libraries.Shared.Pubsub
{
    public static class PubSubServiceExtension
    {
        public static IServiceCollection AddPubSubService(this IServiceCollection service, string key)
        {
            var pubSubConfiguration = Helpers.EnvironmentVariable<PubSubConfiguration>.GetServiceConfiguration(key);

            if (pubSubConfiguration != null
                && !string.IsNullOrWhiteSpace(pubSubConfiguration.ConnectionString)
                && !string.IsNullOrWhiteSpace(pubSubConfiguration.HubName))
                service.AddSingleton(new PubSubClientService(pubSubConfiguration.ConnectionString, pubSubConfiguration.HubName));


            return service;
        }
    }
}
