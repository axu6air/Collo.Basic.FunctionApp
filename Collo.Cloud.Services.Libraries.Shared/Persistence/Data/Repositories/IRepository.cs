using Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Entities;
using Microsoft.Azure.Cosmos;
using System.Linq.Expressions;
using System.Net;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Repositories
{
    public interface IRepository<T> where T : IContainerEntity
    {
        Container Container { get; }

        ValueTask<(List<T> results, HttpStatusCode statusCode)> GetAsync(
            List<string> ids,
            CancellationToken cancellationToken = default);

        ValueTask<(List<T> results, HttpStatusCode statusCode)> GetAsync(
            IReadOnlyList<(string, PartitionKey)> ids,
            CancellationToken cancellationToken = default);

        ValueTask<(T result, HttpStatusCode statusCode)> GetAsync(
               string id,
               string partitionKeyValue = null,
               CancellationToken cancellationToken = default);

        ValueTask<(T result, HttpStatusCode statusCode)> GetAsync(
            string id,
            PartitionKey partitionKey,
            CancellationToken cancellationToken = default);

        ValueTask<(List<T> results, HttpStatusCode statusCode)> GetAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default);

        ValueTask<(List<T> results, HttpStatusCode statusCode)> GetByQueryAsync(
            string query,
            CancellationToken cancellationToken = default);

        ValueTask<(List<T> results, HttpStatusCode statusCode)> GetByQueryAsync(
            QueryDefinition queryDefinition,
            CancellationToken cancellationToken = default);

        ValueTask<(T result, HttpStatusCode statusCode)> CreateAsync(
            T value,
            CancellationToken cancellationToken = default);

        ValueTask<List<(T result, HttpStatusCode statusCode)>> CreateAsync(
            List<T> values,
            CancellationToken cancellationToken = default);

        ValueTask<(T result, HttpStatusCode statusCode)> PatchAsync(
            string id,
            IReadOnlyList<PatchOperation> patchOperations,
            PatchItemRequestOptions patchItemRequestOptions = default,
            PartitionKey partitionKey = default,
            CancellationToken cancellationToken = default,
            bool ignoreEtag = false);

        ValueTask<(T result, HttpStatusCode statusCode)> UpdateAsync(
            T value,
            CancellationToken cancellationToken = default,
            bool isUpdate = true,
            bool ignoreEtag = false);

        ValueTask<List<(T result, HttpStatusCode statusCode)>> UpdateAsync(
             List<T> values,
             CancellationToken cancellationToken = default,
             bool ignoreEtag = false);

        ValueTask<HttpStatusCode> DeleteAsync(
            T value,
            CancellationToken cancellationToken = default);

        ValueTask<HttpStatusCode> DeleteAsync(
            string id,
            string partitionKeyValue = null,
            CancellationToken cancellationToken = default);

        ValueTask<HttpStatusCode> DeleteAsync(
            string id,
            PartitionKey partitionKey,
            CancellationToken cancellationToken = default);


        ValueTask<(bool exist, HttpStatusCode statusCode)> ExistsAsync(string id, string partitionKeyValue = null,
            CancellationToken cancellationToken = default);

        ValueTask<(bool exist, HttpStatusCode statusCode)> ExistsAsync(string id, PartitionKey partitionKey,
            CancellationToken cancellationToken = default);

        ValueTask<(bool exist, HttpStatusCode statusCode)> Exists(Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default);

        ValueTask<(int count, HttpStatusCode statusCode)> CountAsync(CancellationToken cancellationToken = default);

        ValueTask<(int count, HttpStatusCode statusCode)> CountAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default);

        ValueTask<HttpStatusCode> SoftDeleteAsync(
            T value);

        ValueTask<(List<(T result, HttpStatusCode statusCode)> results, string continuationToken)> GetPageAsync(IQueryable<T> query,
             int pageSize,
             string continuationToken = null,
             CancellationToken cancellationToken = default);

        ValueTask<IPage<T>> PageAsync(
          Expression<Func<T, bool>> predicate = null,
          int pageSize = 25,
          string continuationToken = null,
          bool returnTotal = false,
          CancellationToken cancellationToken = default);

        ValueTask<(T result, HttpStatusCode statusCode)> ReplaceAsync(
            T value,
            CancellationToken cancellationToken = default);

        ValueTask<IPageQueryResult<T>> PageAsync(
         Expression<Func<T, bool>> predicate = null,
         int pageNumber = 1,
         int pageSize = 25,
         string continuationToken = null,
         bool returnTotal = false,
         CancellationToken cancellationToken = default);
    }
}
