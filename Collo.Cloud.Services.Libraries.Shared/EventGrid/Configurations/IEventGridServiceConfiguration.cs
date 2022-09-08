namespace Collo.Cloud.Services.Libraries.Shared.Configurations
{
    public interface IEventGridServiceConfiguration
    {
        public string AzureEventGridTopicEndpoint { get; set; }
        public string AzureEventGridTopicAccessKey { get; set; }
    }
}
