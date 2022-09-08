using Collo.Cloud.Services.Libraries.Shared.Analytics.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Collo.Cloud.Services.Libraries.Shared.Analytics
{
    public static class AnalyticsServiceExtension
    {
        /// <summary>
        /// Used for injecting Collo Analytics Service Config
        /// </summary>
        /// <param name="key">is Environment Variable Name</param>
        /// <returns></returns>
        public static IServiceCollection AddAnalyticsConfigurationService(this IServiceCollection service, string key)
        {
            //Get Value from Environment (Deserialize from escaped JSON string)
            var analyticsConfig = Helpers.EnvironmentVariable<AnalyticsServiceConfiguration>.GetServiceConfiguration(key);

            //Configure Service Using Values from Pervious Step
            service.Configure<AnalyticsServiceConfiguration>(x =>
            {
                x.TimeboxLength = analyticsConfig.TimeboxLength;
                x.AdvanceBucketCreationTime = analyticsConfig.AdvanceBucketCreationTime;
                x.BucketResolution = analyticsConfig.BucketResolution;
                x.AggregateResolution = analyticsConfig.AggregateResolution;
                x.MarginOfError = analyticsConfig.MarginOfError;
            });

            //Validations and Dependency Injection
            var serviceProvider = service.BuildServiceProvider();
            var analyticsServiceConfiguration = serviceProvider.GetRequiredService<IOptions<AnalyticsServiceConfiguration>>().Value;
            
            service.AddSingleton<IAnalyticsServiceConfiguration>(analyticsServiceConfiguration);

            return service;
        }
    }
}
