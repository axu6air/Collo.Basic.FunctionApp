using Microsoft.Extensions.DependencyInjection;

namespace Collo.Cloud.Services.Libraries.Shared.EventHub
{
    public static class EventHubServiceExtension
    {
        /// <summary>
        /// Used for injecting Azure Event Hub service
        /// </summary>
        /// <param name="key">is Environment Variable Name</param>
        /// <returns></returns>
        public static IServiceCollection AddFeatureEventHubService(this IServiceCollection service, string key)
        {
            service.AddSingleton(new FeatureEventHubService(key));
            return service;
        }

        public static IServiceCollection AddAccumulateEventHubService(this IServiceCollection service, string key)
        {
            service.AddSingleton(new AccumulateEventHubService(key));
            return service;
        }

        public static IServiceCollection AddTimeseriesEventHubService(this IServiceCollection service, string key)
        {
            service.AddSingleton(new TimeseriesEventHubService(key));
            return service;
        }
    }
}
