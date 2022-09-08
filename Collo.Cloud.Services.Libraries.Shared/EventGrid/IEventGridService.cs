using Azure.Messaging;
using Azure.Messaging.EventGrid.SystemEvents;

namespace Collo.Cloud.Services.Libraries.Shared.EventGrid
{
    public interface IEventGridService
    {
        Task PublishTopicWithHttpClientAsync(CloudEvent eventPayload);
        Task PublishTopicAsync(CloudEvent eventPayload);
        Task PublishTopicAsync(string source, string subject, string eventType, object eventData);
        Task PublishTopicAsync(string source, string subject, string eventType, object eventData, string id);
        SubscriptionValidationResponse ValidateWebhook(CloudEvent payload);

    }
}
