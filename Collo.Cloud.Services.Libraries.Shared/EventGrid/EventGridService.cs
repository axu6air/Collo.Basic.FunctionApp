using Azure;
using Azure.Messaging;
using Azure.Messaging.EventGrid;
using Azure.Messaging.EventGrid.SystemEvents;
using Collo.Cloud.Services.Libraries.Shared.Configurations;
using Newtonsoft.Json;
using System.Text;

namespace Collo.Cloud.Services.Libraries.Shared.EventGrid
{
    public class EventGridService : IEventGridService
    {

        private readonly string _eventGridEndPoint;
        private readonly string _topicAccessKey;

        private readonly EventGridPublisherClient _eventPublisherClient;
        private readonly IEventGridServiceConfiguration _eventGridServiceConfiguration;
        private readonly IHttpClientFactory _httpClientFactory;

        public EventGridService(IHttpClientFactory httpClientFactory, IEventGridServiceConfiguration eventGridServiceConfiguration)
        {
            _eventGridServiceConfiguration = eventGridServiceConfiguration;
            _httpClientFactory = httpClientFactory;
            _eventGridEndPoint = _eventGridServiceConfiguration.AzureEventGridTopicEndpoint;
            _topicAccessKey = _eventGridServiceConfiguration.AzureEventGridTopicAccessKey;

            var azureKeyCredential = new AzureKeyCredential(_topicAccessKey);
            _eventPublisherClient = new EventGridPublisherClient(new Uri(_eventGridEndPoint), azureKeyCredential);
        }

        public async Task PublishTopicAsync(CloudEvent eventPayload)
        {
            List<CloudEvent> payloads = new();
            payloads.Add(eventPayload);

            await _eventPublisherClient.SendEventsAsync(payloads);
        }

        public async Task PublishTopicAsync(string source, string subject, string eventType,
            object eventData)
        {
            List<CloudEvent> payloads = new()
            {
                new CloudEvent(source, eventType, eventData, eventData.GetType())
                {
                    Id = Guid.NewGuid().ToString(),
                    Subject = subject,
                }
            };

            await _eventPublisherClient.SendEventsAsync(payloads);
        }

        public async Task PublishTopicAsync(string source, string subject, string eventType,
            object eventData, string id)
        {
            List<CloudEvent> payloads = new()
            {
                new CloudEvent(source, eventType, eventData, eventData.GetType())
                {
                    Id = id,
                    Subject = subject,
                    Time = DateTime.Now,
                }
            };

            await _eventPublisherClient.SendEventsAsync(payloads);
        }

        public async Task PublishTopicWithHttpClientAsync(CloudEvent eventPayload)
        {
            List<CloudEvent> payloads = new();
            payloads.Add(eventPayload);

            var client = _httpClientFactory.CreateClient();
            var httpContent = new StringContent(JsonConvert.SerializeObject(payloads), Encoding.UTF8, "application/json");

            await client.PostAsync(_eventGridEndPoint, httpContent);
        }

        public SubscriptionValidationResponse ValidateWebhook(CloudEvent payload)
        {
            var response = JsonConvert.DeserializeObject<SubscriptionValidationEventData>(payload.Data.ToString());

            return new SubscriptionValidationResponse()
            {
                ValidationResponse = response.ValidationCode
            };
        }
    }
}
