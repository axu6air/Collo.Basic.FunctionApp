using Collo.Cloud.Services.Libraries.Shared.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Collo.Cloud.Services.Libraries.Shared.EventGrid
{
    public static class EventGirdServiceExtension
    {
        /// <summary>
        /// Used for injecting Azure Event Grid service
        /// </summary>
        /// <param name="key">is Environment Variable Name</param>
        /// <returns></returns>
        public static IServiceCollection AddEventGrid(this IServiceCollection service, string key)
        {
            var serviceProvider = service.BuildServiceProvider();

            //Get Value from Environment
            var eventGridConfig = Helpers.EnvironmentVariable<EventGridServiceConfiguration>.GetServiceConfiguration(key);

            //Configure Service Using Values from Pervious Step
            service.Configure<EventGridServiceConfiguration>(x =>
            {
                x.AzureEventGridTopicEndpoint = eventGridConfig.AzureEventGridTopicEndpoint;
                x.AzureEventGridTopicAccessKey = eventGridConfig.AzureEventGridTopicAccessKey;
            });

            service.AddSingleton<IValidateOptions<EventGridServiceConfiguration>, EventGridServiceConfigurationValidation>();
            var eventGridServiceConfiguration = serviceProvider.GetRequiredService<IOptions<EventGridServiceConfiguration>>().Value;
            service.AddSingleton<IEventGridServiceConfiguration>(eventGridServiceConfiguration);

            service.AddHttpClient();
            service.AddTransient<IEventGridService, EventGridService>();

            return service;
        }
    }
}
