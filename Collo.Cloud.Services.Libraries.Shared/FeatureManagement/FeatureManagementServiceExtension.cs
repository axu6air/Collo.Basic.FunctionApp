using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;

namespace Collo.Cloud.Services.Libraries.Shared.FeatureManagements
{
    public static class FeatureManagementServiceExtension
    {
        public static IServiceCollection AddFeatureManagementService(this IServiceCollection service, IConfiguration configuration)
        {
            var serviceProvider = service.BuildServiceProvider();
            service.AddFeatureManagement(configuration.GetSection("COLLOFEATUREMANAGEMENT_KV"))
                           .AddFeatureFilter<PercentageFilter>();

            return service;
        }
    }
}
