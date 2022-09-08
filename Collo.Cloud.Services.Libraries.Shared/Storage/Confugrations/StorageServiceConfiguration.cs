namespace Collo.Cloud.Templates.Shared.Configurations
{
    public class StorageServiceConfiguration : IStorageServiceConfiguration
    {
        public string ContainerName { get; set; }
        public string ConnectionString { get; set; }
    }
}
