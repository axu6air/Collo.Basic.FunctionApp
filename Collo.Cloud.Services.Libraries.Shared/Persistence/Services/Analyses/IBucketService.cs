using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Analysis;
using Microsoft.Azure.Cosmos;
using System.Linq.Expressions;
using System.Net;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Analyses
{
    public interface IBucketService
    {
        ValueTask<Bucket> CreateBucketAsync(Bucket bucket);

        ValueTask<Bucket> GetBucketAsync(
            DateTime measuredAt,
            string feature,
            string instrumentId);

        ValueTask<List<Bucket>> GetBucketAsync(
            string query);

        ValueTask<(Bucket, HttpStatusCode)> PatchBucketAsync(
            string bucketId,
            IReadOnlyList<PatchOperation> patchOperations,
            PartitionKey partitionKey = default);

        ValueTask<List<Bucket>> GetBucketAsync(
            Expression<Func<Bucket, bool>> predicate,
            CancellationToken cancellationToken = default);

        ValueTask<Bucket> GetBucketByIdAsync(string id, string partitionKey);
        //ValueTask PatchBucketAsync(DateTime measuredAt, int time, float value, string feature, string instrumentId);
    }
}
