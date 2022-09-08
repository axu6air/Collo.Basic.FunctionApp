using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

namespace Collo.Cloud.Services.Libraries.Shared.EventHub
{
    public interface IEventHubService
    {
        Task<EventDataBatch> CreateBatchAsync();
        Task ProduceBatchEventAsync(
            EventDataBatch eventDataBatch,
            CancellationToken cancellationToken = default);
        Task ProduceEventAsync(EventData eventData);
        Task ProduceEventAsync(BinaryData eventBody);
        Task ProduceEventAsync(string partitionId, EventData eventData);
        Task<List<string>> ReadEventsAsync(string taskCancelTime);
        Task<List<string>> ReadEventFromPartition(string partitionId, string taskCancelTime);
    }
}
