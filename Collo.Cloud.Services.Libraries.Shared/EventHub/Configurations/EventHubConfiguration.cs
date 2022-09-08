namespace Collo.Cloud.Services.Libraries.Shared.EventHub.Configurations
{
    public class EventHubConfiguration
    {
        public string ConnectionString { get; set; }
        public string EventHubName { get; set; }
        public string ConsumerGroup { get; set; }
    }
}
