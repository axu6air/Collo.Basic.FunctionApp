namespace Collo.Cloud.Services.Libraries.Shared.Configurations
{
    public class EventGridServiceConfiguration : IEventGridServiceConfiguration
    {
        public string AzureEventGridTopicEndpoint { get; set; }
        public string AzureEventGridTopicAccessKey { get; set; }
    }
}
