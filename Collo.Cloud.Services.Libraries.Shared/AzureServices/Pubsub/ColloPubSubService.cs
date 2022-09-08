using Azure.Messaging.WebPubSub;

namespace Collo.Cloud.Services.Libraries.Shared.AzureServices.Pubsub
{
    public class PubSubClientService : WebPubSubServiceClient
    {
        public PubSubClientService(string connectionString, string hubName) : base(connectionString, hubName)
        {

        }
    }
}
