namespace Collo.Cloud.Templates.Shared.Configurations
{
    public interface IStorageServiceConfiguration
    {
        public string ContainerName { get; set; }
        public string ConnectionString { get; set; }
    }
}
