namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Configurations
{

    public interface ICosmosDbDataServiceConfiguration
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string Budget { get; set; }
        string MaxRetryAttempts { get; set; }
        string MaxRetryWaitTime { get; set; }

        List<CosmosContainer> Containers { get; set; }
    }
}
