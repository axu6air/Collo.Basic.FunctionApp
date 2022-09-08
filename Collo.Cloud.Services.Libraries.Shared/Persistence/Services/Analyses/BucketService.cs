using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Analysis;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Repositories;
using Humanizer;
using Microsoft.Azure.Cosmos;
using System.Linq.Expressions;
using System.Net;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Analyses
{
    public class BucketService : IBucketService
    {
        private readonly IRepository<Bucket> _bucketRepository;
        public BucketService(IRepository<Bucket> bucketRepository)
        {
            _bucketRepository = bucketRepository;
        }

        public async ValueTask<Bucket> CreateBucketAsync(Bucket bucket)
        {
            var createdBucket = await _bucketRepository.CreateAsync(bucket);

            if (createdBucket.statusCode == System.Net.HttpStatusCode.OK)
                return createdBucket.result;

            return null;
        }


        public async ValueTask<List<Bucket>> GetBucketAsync(
            string query)
        {
            var (buckets, httpStatusCode) = await _bucketRepository.GetByQueryAsync(query);

            if (httpStatusCode == HttpStatusCode.OK && buckets.Count > 0)
                return buckets;

            return null;
        }

        public async ValueTask<Bucket> GetBucketByIdAsync(string id, string partitionKey)
        {
            var (bucket, httpStatusCode) = await _bucketRepository.GetAsync(id, partitionKey: new PartitionKey(partitionKey));

            if (httpStatusCode == HttpStatusCode.OK)
                return bucket;

            return null;
        }

        /// <summary>
        /// Get the bucket in which times and values will be patched
        /// </summary>
        /// <param name="measuredAt">MeasuredAt UTC of the feature</param>
        /// <param name="feature">Name of the feature</param>
        /// <param name="instrumentId">Instrument Id</param>
        /// <returns>Bucket</returns>
        public async ValueTask<Bucket> GetBucketAsync(
            DateTime measuredAt,
            string feature,
            string instrumentId)
        {
            var (buckets, httpStatusCode) = await _bucketRepository.GetAsync
                (x => x.StartTime <= measuredAt && x.EndTime >= measuredAt
                && x.Feature == feature
                && x.Id.StartsWith(instrumentId));

            if (httpStatusCode == HttpStatusCode.OK && buckets.Count > 0)
                return buckets.FirstOrDefault();

            return null;
        }

        /// <summary>
        /// Get bucket through query expression
        /// </summary>
        /// <param name="predicate">Query Expression</param>
        /// <param name="cancellationToken">Cosmos DB Cancellation Token, Initialized with default value if not passed</param>
        /// <returns>Bucket</returns>
        public async ValueTask<List<Bucket>> GetBucketAsync(
            Expression<Func<Bucket, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            var (buckets, httpStatusCode) = await _bucketRepository.GetAsync(predicate, cancellationToken);

            if (httpStatusCode == HttpStatusCode.OK && buckets.Count > 0)
                return buckets;

            return null;
        }

        /// <summary>
        /// Patch bucket, use to add 'times' and 'values'
        /// </summary>
        /// <param name="bucketId">Bucket Id</param>
        /// <param name="patchOperations">Patch Operations with paths and values</param>
        /// <param name="partitionKey">Container partition key</param>
        /// <returns>Bucket if patch successful else 'null'</returns>
        public async ValueTask<(Bucket, HttpStatusCode)> PatchBucketAsync(
            string bucketId,
            IReadOnlyList<PatchOperation> patchOperations,
            PartitionKey partitionKey = default)
        {
            var (bucket, httpStatusCode) = await _bucketRepository.PatchAsync(
                bucketId,
                patchOperations,
                partitionKey: partitionKey);

            return (bucket, httpStatusCode);
        }

        private async ValueTask<List<Bucket>> GetPartitionedBucketAsync(string id)
        {
            var splittedId = Helpers.IdHelper.IdSplitter(id);

            var (buckets, httpStatusCode) = await _bucketRepository.GetByQueryAsync($"SELECT " +
                $"x.{nameof(Bucket.Id).Camelize()}," +
                $"x.{nameof(Bucket.PartitionKey).Camelize()}," +
                $"x.{nameof(Bucket.StartTime).Camelize()}," +
                $"x.{nameof(Bucket.EndTime).Camelize()}," +
                $"x.{nameof(Bucket.Count).Camelize()}," +
                $"x.{nameof(Bucket.Ttl).Camelize()}," +
                $"x.{nameof(Bucket.Schema).Camelize()} FROM {nameof(Bucket)} x WHERE CONTAINS(x.{nameof(Bucket.Id).Camelize()}\"{splittedId}\")");

            return buckets;
        }

        public async ValueTask PatchBucketAsync(DateTime measuredAt, int time, float value, string feature, string instrumentId)
        {
            //var partitionedBuckets = await GetPartitionedBucketAsync(id);
            //var partitionedLastBucket = partitionedBuckets.LastOrDefault();
            //var splitterCounter = Helpers.IdHelper.IdSplitterCounter(partitionedLastBucket.Id);

            //await Policy.Handle<CosmosException>(e => e.StatusCode == HttpStatusCode.RequestEntityTooLarge)
            //    .RetryForeverAsync(onRetry: async (e) =>
            //    {
            //        //Split Document
            //        //Take the counter from "splitterCounter" increment by 1 and make new Splitted Bucket Document
            //        //Patch Newly created document (insert times and values)
            //    })
            //    .ExecuteAsync(async () =>
            //    {
            //        await _bucketRepository.Container.PatchItemAsync<Bucket>(
            //            id,
            //            new PartitionKey(id),
            //            new[]
            //            {
            //                PatchOperation.Add("/times/-", time),
            //                PatchOperation.Add("/values/-", value)
            //            }
            //        );

            //    });
            //var query = $"SELECT x.{nameof(Bucket.Id).Camelize()}," +
            //    $" x.{nameof(Bucket.Count).Camelize()}," +
            //    $" x.{nameof(Bucket.Values).Camelize()}" +
            //    $" FROM x" +
            //    $" WHERE x.startTime <= {measuredAt}" +
            //    $" AND x.endTime >= {measuredAt}" +
            //    $" AND x.feature = '{feature}'";

            var (buckets, httpStatusCode) = await _bucketRepository.GetAsync
                (x => x.StartTime <= measuredAt && x.EndTime >= measuredAt
                && x.Feature == feature
                && x.Id.StartsWith(instrumentId));
            //var (bucket, httpStatusCode) = await _bucketRepository.GetAsync(x => x.Feature == feature);
            var bucket = buckets.FirstOrDefault();

            //if(firstBucket == null)
            //{
            //    //Create New Bucket Logic
            //}

            if (bucket != null && bucket.Count == bucket.Values.Length)
            {
                //Notify buckets ready
            }
            else if (bucket != null)
            {
                await _bucketRepository.PatchAsync(bucket.Id,
                new[]
                {
                    PatchOperation.Add("/times/-", time),
                    PatchOperation.Add("/values/-", value)
                }, partitionKey: new PartitionKey(bucket.PartitionKey)
            );
            }
        }
    }
}