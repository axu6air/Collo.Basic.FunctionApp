using Collo.Cloud.Services.Libraries.Shared.EventHub.Configurations;

namespace Collo.Cloud.Services.Libraries.Shared.Helpers
{
    public class EventHubHelpers
    {
        public static EventHubConfiguration GetEventHubConfigurations(string key)
        {
            var config = EnvironmentVariable<EventHubConfiguration>.GetServiceConfiguration(key);
            return config;
        }
    }
}
