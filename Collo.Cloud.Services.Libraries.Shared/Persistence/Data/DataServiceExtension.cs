using Collo.Cloud.Services.Libraries.Shared.Commons;
using Collo.Cloud.Services.Libraries.Shared.PerformanceMetrics;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Configurations;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Entities;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Data
{
    public static class DataServiceExtension
    {
        /// <summary>
        /// Used for injecting Cosmos DB service
        /// </summary>
        /// <param name="key">is Environment Variable Name</param>
        /// <returns></returns>
        public static IServiceCollection AddDataConfigurationService(this IServiceCollection service, string key)
        {
            var logger = LoggerCore.GetLogger("DataServiceExtension");

            logger.LogInformation("Cosmos db initiating...");

            var cosmosConfig = Helpers.EnvironmentVariable<CosmosDbDataServiceConfiguration>.GetServiceConfiguration(key);

            logger.LogInformation("Cosmos db config");
            logger.LogInformation(JsonConvert.SerializeObject(cosmosConfig));

            #region Dependency Injections 

            logger.LogInformation($"Cosmos db services trying to inject...");
            service.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            service.AddTransient(typeof(IPage<>), typeof(Page<>));
            service.AddTransient(typeof(IPageQueryResult<>), typeof(PageQueryResult<>));
            service.AddSingleton<IPerformanceMetricsService, PerformanceMetricsService>();

            logger.LogInformation($"Cosmos db services injected...");
            #endregion

            try
            {
                var cosmoDbConfiguration = PrepareCosmosClient(service, cosmosConfig, out CosmosClient cosmosClient);
                logger.LogInformation("Cosmos client prepared...");
                int? databaseBudget = int.TryParse(cosmoDbConfiguration.Budget, out int dbBudget) ? dbBudget : null;

                logger.LogInformation("Cosmos db trying to create if not exist...");
                var database = cosmosClient.CreateDatabaseIfNotExistsAsync(cosmoDbConfiguration.DatabaseName, databaseBudget).GetAwaiter().GetResult();

                logger.LogInformation("Cosmos db created or found...");

                //Creating containers dynamically
                if (database != null)
                    foreach (var container in cosmoDbConfiguration.Containers)
                    {
                        int? containerBudget = int.TryParse(container.Budget, out int contBudget) ? contBudget : null;
                        int? defaultTimeToLive = int.TryParse(container.TimeToLive, out int contTimeToLive) ? contTimeToLive : -1;

                        logger.LogInformation($"Cosmos db container {container.Name} created or found...");

                        ContainerProperties containerProperties = new()
                        {
                            Id = container.Name,
                            PartitionKeyPath = container.PartitionKeyPath,
                            DefaultTimeToLive = defaultTimeToLive
                        };
                        _ = database?.Database.CreateContainerIfNotExistsAsync(containerProperties, throughput: containerBudget).GetAwaiter().GetResult();
                    }

                service.AddSingleton(cosmosClient);

                logger.LogInformation("Cosmos db successfully initiated");
            }
            catch (Exception ex)
            {
                logger.LogError("Cosmos db could not be initiated");
                logger.LogError("Cosmos db informations: ");
                logger.LogInformation($"ConnectionString: {cosmosConfig?.ConnectionString}");
                logger.LogInformation($"DatabaseName: {cosmosConfig?.DatabaseName}");
                logger.LogInformation($"Containers: {cosmosConfig?.Containers}");
                logger.LogInformation($"Budget: {cosmosConfig?.Budget}");
                logger.LogInformation($"MaxRetryAttempts: {cosmosConfig?.MaxRetryAttempts}");
                logger.LogInformation($"MaxRetryWaitTime: {cosmosConfig?.MaxRetryWaitTime}");

                logger.LogError(ex, message: ex.Message);
            }

            return service;
        }

        private static ICosmosDbDataServiceConfiguration PrepareCosmosClient(IServiceCollection service, CosmosDbDataServiceConfiguration cosmosConfig, out CosmosClient cosmosClient)
        {
            service.Configure<CosmosDbDataServiceConfiguration>(x =>
            {
                x.ConnectionString = cosmosConfig.ConnectionString;
                x.DatabaseName = cosmosConfig.DatabaseName;
                x.Containers = cosmosConfig.Containers;
                x.Budget = cosmosConfig.Budget;
                x.MaxRetryAttempts = cosmosConfig.MaxRetryAttempts;
                x.MaxRetryWaitTime = cosmosConfig.MaxRetryWaitTime;
            });

            #region Dependency Injections 

            //Validations and Dependency Injection
            service.AddSingleton<IValidateOptions<CosmosDbDataServiceConfiguration>, CosmosDbDataServiceConfigurationValidation>();
            var cosmosDbDataServiceConfiguration = service.BuildServiceProvider().GetRequiredService<IOptions<CosmosDbDataServiceConfiguration>>().Value;
            service.AddSingleton<ICosmosDbDataServiceConfiguration>(cosmosDbDataServiceConfiguration);

            #endregion

            var serviceProvider = service.BuildServiceProvider();
            var cosmoDbConfiguration = serviceProvider.GetRequiredService<ICosmosDbDataServiceConfiguration>();

            CosmosClientOptions cosmosClientOptions = new()
            {
                SerializerOptions = new CosmosSerializationOptions()
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                }
            };

            //Retry policy for 429 StatusCode
            cosmosClientOptions.MaxRetryAttemptsOnRateLimitedRequests = int.TryParse(cosmoDbConfiguration.MaxRetryAttempts, out int maxRetryAttempt) ? maxRetryAttempt : 9;
            cosmosClientOptions.MaxRetryWaitTimeOnRateLimitedRequests = TimeSpan.FromSeconds(double.TryParse(cosmoDbConfiguration.MaxRetryWaitTime, out double maxRetryWait) ? maxRetryWait : 30);

            //Creating cosmos db if not exits
            cosmosClient = new(cosmoDbConfiguration.ConnectionString, cosmosClientOptions);
            return cosmoDbConfiguration;
        }

    }
}
