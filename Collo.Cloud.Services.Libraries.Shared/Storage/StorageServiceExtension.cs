using Azure.Storage.Blobs;
using Collo.Cloud.Templates.Shared.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Collo.Cloud.Services.Libraries.Shared.Storage
{
    public static class StorageServiceExtension
    {
        public static IServiceCollection AddStorage(this IServiceCollection services, IConfiguration configuration)
        {
            var serviceProvider = services.BuildServiceProvider();

            services.Configure<StorageServiceConfiguration>(configuration.GetSection("STORAGESETTINGS_KV"));
            services.AddSingleton<IValidateOptions<StorageServiceConfiguration>, StorageServiceConfigurationValidation>();
            var storageServiceConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<StorageServiceConfiguration>>().Value;
            services.AddSingleton<IStorageServiceConfiguration>(storageServiceConfiguration);

            services.TryAddSingleton(implementationFactory => new BlobServiceClient(storageServiceConfiguration.ConnectionString));

            services.AddSingleton<IStorageService, StorageService>();

            return services;
        }
    }
}
