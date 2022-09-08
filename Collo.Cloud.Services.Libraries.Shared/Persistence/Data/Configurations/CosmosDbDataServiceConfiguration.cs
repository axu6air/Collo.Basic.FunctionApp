namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Configurations
{
    public class CosmosDbDataServiceConfiguration : ICosmosDbDataServiceConfiguration
    {
        public const string CosmosDbSettings = "COSMOSDBCONNECTIONSETTINGS_COMMON_KV";

        public CosmosDbDataServiceConfiguration()
        {
            Containers = new List<CosmosContainer>();
        }

        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string Budget { get; set; }
        public string MaxRetryAttempts { get; set; }
        public string MaxRetryWaitTime { get; set; }

        public List<CosmosContainer> Containers { get; set; }
    }

    public class CosmosContainer
    {
        public string Name { get; set; }
        public string Budget { get; set; }
        public string PartitionKeyPath { get; set; }
        public string TimeToLive { get; set; }
    }

}
