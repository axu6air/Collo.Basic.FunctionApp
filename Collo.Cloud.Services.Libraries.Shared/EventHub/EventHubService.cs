using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Producer;
using Collo.Cloud.Services.Libraries.Shared.Commons;
using Collo.Cloud.Services.Libraries.Shared.Helpers;
using Microsoft.Extensions.Logging;

namespace Collo.Cloud.Services.Libraries.Shared.EventHub
{
    public class EventHubService : IEventHubService
    {
        private readonly EventHubProducerClient _eventHubproducerClient;
        private readonly EventHubConsumerClient _eventHubConsumerClient;
        private readonly ILogger _logger;

        public EventHubService(string key)
        {
            var eventHubConfiguration = EventHubHelpers.GetEventHubConfigurations(key);
            _eventHubproducerClient = new EventHubProducerClient(eventHubConfiguration.ConnectionString, eventHubConfiguration.EventHubName);
            _eventHubConsumerClient = new EventHubConsumerClient(eventHubConfiguration.ConsumerGroup, eventHubConfiguration.ConnectionString,
               eventHubConfiguration.EventHubName);

            _logger = LoggerCore.GetLogger<EventHubService>();
        }

        /// <summary>
        /// Create Event Batch
        /// </summary>
        /// <param name="eventData"></param>
        /// <returns>EventDataBatch</returns>
        public async Task<EventDataBatch> CreateBatchAsync()
        {
            return await _eventHubproducerClient.CreateBatchAsync();
        }

        /// <summary>
        /// Produce Batched Event
        /// </summary>
        /// <param name="eventDataBatch"></param>
        /// <returns></returns>
        public async Task ProduceBatchEventAsync(
            EventDataBatch eventDataBatch,
            CancellationToken cancellationToken = default)
        {
            await _eventHubproducerClient.SendAsync(eventDataBatch, cancellationToken);
        }

        public async Task ProduceEventAsync(EventData eventData)
        {
            using EventDataBatch eventBatch = await _eventHubproducerClient.CreateBatchAsync();
            eventBatch.TryAdd(eventData);

            await _eventHubproducerClient.SendAsync(eventBatch);
        }

        public async Task ProduceEventAsync(BinaryData eventBody)
        {
            using EventDataBatch eventBatch = await _eventHubproducerClient.CreateBatchAsync();
            eventBatch.TryAdd(new EventData(eventBody));

            await _eventHubproducerClient.SendAsync(eventBatch);
        }

        public async Task ProduceEventAsync(string partitionId, EventData eventData)
        {
            var batchOptions = new CreateBatchOptions
            {
                PartitionId = partitionId,
            };

            using EventDataBatch eventBatch = await _eventHubproducerClient.CreateBatchAsync(batchOptions);
            eventBatch.TryAdd(eventData);

            await _eventHubproducerClient.SendAsync(eventBatch);
        }

        public async Task<List<string>> ReadEventsAsync(string taskCancelTime)
        {
            var readEventsList = new List<string>();

            try
            {
                int cancelTime = Convert.ToInt32(Environment.GetEnvironmentVariable(taskCancelTime));

                if (cancelTime == 0)
                {
                    _logger.LogInformation("Task cancel time must be greater than 0");

                    return null;
                }

                using CancellationTokenSource cancellationSource = new CancellationTokenSource();
                cancellationSource.CancelAfter(TimeSpan.FromSeconds(cancelTime));

                var readEvents = _eventHubConsumerClient.ReadEventsAsync(cancellationSource.Token);

                await foreach (PartitionEvent partitionEvent in readEvents)
                {
                    var readeventString = partitionEvent.Data.EventBody.ToString();

                    readEventsList.Add(readeventString);
                }

                return readEventsList;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogInformation(ex, ex.Message);

                return readEventsList;
            }
            catch (Exception ex) when (ex is not TaskCanceledException)
            {
                _logger.LogInformation(ex, ex.Message);
            }

            return null;
        }

        public async Task<List<string>> ReadEventFromPartition(string partitionId, string taskCancelTime)
        {
            var readEventFromPartitionList = new List<string>();

            try
            {
                int cancelTime = Convert.ToInt32(Environment.GetEnvironmentVariable(taskCancelTime));

                if (cancelTime == 0)
                {
                    _logger.LogInformation("Task cancel time must be greater than 0");

                    return null;
                }

                using CancellationTokenSource cancellationSource = new CancellationTokenSource();
                cancellationSource.CancelAfter(TimeSpan.FromSeconds(cancelTime));

                EventPosition startingPosition = EventPosition.Earliest;

                var readEvent = _eventHubConsumerClient.ReadEventsFromPartitionAsync(partitionId,
                    startingPosition, cancellationSource.Token);

                await foreach (PartitionEvent partitionEvent in readEvent)
                {
                    var readEventString = partitionEvent.Data.EventBody.ToString();

                    readEventFromPartitionList.Add(readEventString);
                }
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogInformation(ex, ex.Message);

                return readEventFromPartitionList;
            }
            catch (Exception ex) when (ex is not TaskCanceledException)
            {
                _logger.LogInformation(ex, ex.Message);
            }

            return null;
        }
    }
}

