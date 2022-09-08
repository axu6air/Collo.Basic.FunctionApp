using Collo.Cloud.Services.Libraries.Shared.CustomExceptionHandlers;
using Collo.Cloud.Services.Libraries.Shared.Helpers;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Configurations;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Entities;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Linq.Expressions;
using System.Net;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Repositories
{
    public class Repository<T> : IRepository<T> where T : IContainerEntity
    {
        private readonly Container _container;

        public Repository(CosmosClient client, ICosmosDbDataServiceConfiguration dataServiceConfiguration)
        {
            _container = client.GetContainer(dataServiceConfiguration.DatabaseName, typeof(T).Name);
        }

        public Container Container
        {
            get
            {
                return _container;
            }
        }

        public async ValueTask<(List<T> results, HttpStatusCode statusCode)> GetAsync(
           List<string> ids,
           CancellationToken cancellationToken = default)
        {
            List<(string, PartitionKey)> items = new List<(string, PartitionKey)>();

            foreach (var id in ids)
            {
                items.Add((id, new PartitionKey(id)));
            }

            IReadOnlyList<(string, PartitionKey)> values = items;

            return await GetManyAsync(values, cancellationToken);
        }

        public async ValueTask<(List<T> results, HttpStatusCode statusCode)> GetAsync(
            IReadOnlyList<(string, PartitionKey)> ids,
            CancellationToken cancellationToken = default) => await GetManyAsync(ids, cancellationToken);

        private async ValueTask<(List<T> results, HttpStatusCode statusCode)> GetManyAsync(
            IReadOnlyList<(string, PartitionKey)> ids,
            CancellationToken cancellationToken
            )
        {
            var response = await _container.ReadManyItemsAsync<T>(items: ids, cancellationToken: cancellationToken)
                          .ConfigureAwait(false);

            return (response.ToList(), response.StatusCode);
        }

        public async ValueTask<(T result, HttpStatusCode statusCode)> GetAsync(
            string id,
            string partitionKeyValue = null,
            CancellationToken cancellationToken = default) =>
            await GetAsync(id, new PartitionKey(partitionKeyValue ?? id), cancellationToken);

        public async ValueTask<(T result, HttpStatusCode statusCode)> GetAsync(
             string id,
             PartitionKey partitionKey,
             CancellationToken cancellationToken = default)
        {
            try
            {
                if (partitionKey == default)
                    partitionKey = new PartitionKey(id);

                ItemResponse<T> response = await _container.ReadItemAsync<T>(id, partitionKey, cancellationToken: cancellationToken)
                                           .ConfigureAwait(false);

                T item = response.Resource;

                return (item is { Type.Length: 0 } || item.Type == typeof(T).Name ? item : default, response.StatusCode);
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new NullEntityException($"{typeof(T).Name} for {id} is not found");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async ValueTask<(List<T> results, HttpStatusCode statusCode)> GetAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var results = new List<T>();

                var query = _container.GetItemLinqQueryable<T>(
                              requestOptions: new QueryRequestOptions
                              {
                                  PartitionKey = new PartitionKey("/id"),
                                  MaxItemCount = 100000
                              }
                          )
                          .Where(predicate);

                var feedIterator = _container.GetItemQueryIterator<T>(query.ToQueryDefinition());


                while (feedIterator.HasMoreResults)
                {
                    var data = await feedIterator.ReadNextAsync(cancellationToken);
                    results.AddRange(data);
                }

                return (results, HttpStatusCode.OK);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound || ex.StatusCode == HttpStatusCode.BadRequest)
            {
                return (new List<T>(), ex.StatusCode);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async ValueTask<(List<T> results, HttpStatusCode statusCode)> GetByQueryAsync(
            string query,
            CancellationToken cancellationToken = default)
        {
            QueryDefinition queryDefinition = new(query);

            return await GetByQueryAsync(queryDefinition, cancellationToken);
        }

        public async ValueTask<(List<T> results, HttpStatusCode statusCode)> GetByQueryAsync(
            QueryDefinition queryDefinition,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var queryResultSetIterator = _container.GetItemQueryIterator<T>(queryDefinition);

                List<T> entities = new();

                while (queryResultSetIterator.HasMoreResults)
                {
                    var response = await queryResultSetIterator.ReadNextAsync(cancellationToken);
                    foreach (var item in response)
                        entities.Add(item);
                }

                return (entities, HttpStatusCode.OK);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound || ex.StatusCode == HttpStatusCode.BadRequest)
            {
                return (new List<T>(), ex.StatusCode);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async ValueTask<(T result, HttpStatusCode statusCode)> CreateAsync(
            T value,
            CancellationToken cancellationToken = default)
        {
            value = GenerateCommonBaseEntity(value, operation: "Create");

            var response =
                 await _container.CreateItemAsync(value, new PartitionKey(value.PartitionKey),
                         cancellationToken: cancellationToken);

            return (response.Resource, response.StatusCode);
        }

        public async ValueTask<List<(T result, HttpStatusCode statusCode)>> CreateAsync(
            List<T> values,
            CancellationToken cancellationToken = default)
        {
            List<(T item, HttpStatusCode statusCode)> result = new();

            foreach (var value in values)
            {
                result.Add(await CreateAsync(value, cancellationToken));
            }

            return result;
        }

        public async ValueTask<(T result, HttpStatusCode statusCode)> ReplaceAsync(
            T value,
            CancellationToken cancellationToken = default)
        {
            var response =
                 await _container.ReplaceItemAsync<T>(value, value.Id, new PartitionKey(value.PartitionKey),
                         cancellationToken: cancellationToken);

            return (response.Resource, response.StatusCode);
        }

        public async ValueTask<(T result, HttpStatusCode statusCode)> PatchAsync(
            string id,
            IReadOnlyList<PatchOperation> patchOperations,
            PatchItemRequestOptions patchItemRequestOptions = default,
            PartitionKey partitionKey = default,
            CancellationToken cancellationToken = default,
            bool ignoreEtag = false)
        {
            try
            {
                if (partitionKey == default)
                    partitionKey = new PartitionKey(id);

                ItemResponse<T> response =
                    await _container.PatchItemAsync<T>(id, partitionKey, patchOperations, patchItemRequestOptions, cancellationToken)
                    .ConfigureAwait(false);

                return (response.Resource, response.StatusCode);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async ValueTask<(T result, HttpStatusCode statusCode)> UpdateAsync(
            T value,
            CancellationToken cancellationToken = default,
            bool isUpdate = true,
            bool ignoreEtag = false)
        {
            value = GenerateCommonBaseEntity(value, operation: isUpdate ? "Update" : "Delete");

            ItemResponse<T> response =
                await _container.UpsertItemAsync<T>(value, new PartitionKey(value.PartitionKey), null,
                        cancellationToken)
                    .ConfigureAwait(false);

            return (response.Resource, response.StatusCode);
        }

        public async ValueTask<List<(T result, HttpStatusCode statusCode)>> UpdateAsync(
            List<T> values,
            CancellationToken cancellationToken = default,
            bool ignoreEtag = false)
        {

            var updateTasks = new List<(T item, HttpStatusCode statusCode)>();

            foreach (var value in values)
                updateTasks.Add(await UpdateAsync(value, cancellationToken));

            return updateTasks;
        }

        public ValueTask<HttpStatusCode> DeleteAsync(
            T value,
            CancellationToken cancellationToken = default) =>
            DeleteAsync(value.Id, value.PartitionKey, cancellationToken);

        public async ValueTask<HttpStatusCode> DeleteAsync(
            string id,
            string partitionKeyValue = null,
            CancellationToken cancellationToken = default) =>
            await DeleteAsync(id, new PartitionKey(partitionKeyValue ?? id), cancellationToken);

        public async ValueTask<HttpStatusCode> DeleteAsync(
            string id,
            PartitionKey partitionKey,
            CancellationToken cancellationToken = default)
        {

            if (partitionKey == default)
                partitionKey = new PartitionKey(id);

            var response = await _container.DeleteItemAsync<T>(id, partitionKey, null, cancellationToken)
                .ConfigureAwait(false);

            return (response.StatusCode);

        }

        public async ValueTask<HttpStatusCode> SoftDeleteAsync(
            T value)
        {
            var type = typeof(T).GetInterface(nameof(ISoftDeleteEnabledEntity));

            if (type == null)
                return HttpStatusCode.NotFound;

            var deleteProp = type.GetProperty("IsDeleted");
            deleteProp.SetValue(value, true);
            var data = await UpdateAsync(value);

            return data.statusCode;
        }

        public ValueTask<(bool exist, HttpStatusCode statusCode)> ExistsAsync(string id, string partitionKeyValue = null,
            CancellationToken cancellationToken = default)
            => ExistsAsync(id, new PartitionKey(partitionKeyValue ?? id), cancellationToken);

        public async ValueTask<(bool exist, HttpStatusCode statusCode)> ExistsAsync(string id, PartitionKey partitionKey,
            CancellationToken cancellationToken = default)
        {
            if (partitionKey == default)
                partitionKey = new PartitionKey(id);

            try
            {
                var response = await _container.ReadItemAsync<T>(id, partitionKey, cancellationToken: cancellationToken)
                                   .ConfigureAwait(false);
            }
            catch (CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return (false, e.StatusCode);
            }

            return (true, HttpStatusCode.OK);
        }

        public async ValueTask<(bool exist, HttpStatusCode statusCode)> Exists(Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query =
                _container.GetItemLinqQueryable<T>()
                    .Where(predicate);

            var response = await query.CountAsync(cancellationToken);

            return response.Resource > 0 ? (true, response.StatusCode) : (false, response.StatusCode);
        }

        public async ValueTask<(int count, HttpStatusCode statusCode)> CountAsync(CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _container.GetItemLinqQueryable<T>();

            var response = await query.CountAsync(cancellationToken: cancellationToken);

            return (response.Resource, response.StatusCode);
        }

        public async ValueTask<(int count, HttpStatusCode statusCode)> CountAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query =
                _container.GetItemLinqQueryable<T>()
                    .Where(predicate);

            var response = await query.CountAsync(cancellationToken);

            return (response.Resource, response.StatusCode);
        }

        public async ValueTask<(List<(T result, HttpStatusCode statusCode)> results, string continuationToken)> GetPageAsync(IQueryable<T> query,
            int pageSize,
            string continuationToken = null,
            CancellationToken cancellationToken = default)
        {
            List<(T result, HttpStatusCode statusCode)> results = new();
            int readItemsCount = 0;
            using FeedIterator<T> iterator = query.ToFeedIterator();

            while (readItemsCount < pageSize && iterator.HasMoreResults)
            {
                FeedResponse<T> response =
                    await iterator.ReadNextAsync(cancellationToken);

                foreach (var item in response)
                    if (readItemsCount != pageSize)
                    {
                        continuationToken = response.ContinuationToken;
                        var result = (item, response.StatusCode);
                        results.Add(result);
                        readItemsCount++;
                    }
            }

            return (results, continuationToken);
        }

        public async ValueTask<IPage<T>> PageAsync(
           Expression<Func<T, bool>> predicate = null,
           int pageSize = 25,
           string continuationToken = null,
           bool returnTotal = false,
           CancellationToken cancellationToken = default)
        {

            QueryRequestOptions options = new()
            {
                MaxItemCount = pageSize
            };

            IQueryable<T> query = _container
                .GetItemLinqQueryable<T>(requestOptions: options, continuationToken: continuationToken)
                .Where(predicate);

            Response<int> countResponse = null;

            if (returnTotal)
            {
                countResponse = await query.CountAsync();
            }

            (List<(T result, HttpStatusCode statusCode)> results, string continuationToken) items = await GetPageAsync(query, pageSize, continuationToken, cancellationToken);

            return (new Page<T>(
                countResponse.Resource,
                pageSize,
                items.results,
                items.continuationToken));
        }

        public async ValueTask<IPageQueryResult<T>> PageAsync(
         Expression<Func<T, bool>> predicate = null,
         int pageNumber = 1,
         int pageSize = 25,
         string continuationToken = null,
         bool returnTotal = false,
         CancellationToken cancellationToken = default)
        {

            IQueryable<T> query = _container
                .GetItemLinqQueryable<T>()
                .Where(predicate);

            Response<int> countResponse = null;

            if (returnTotal)
            {
                countResponse = await query.CountAsync(cancellationToken);
            }

            query = query.Skip(pageSize * (pageNumber - 1))
                .Take(pageSize);


            (List<(T result, HttpStatusCode statusCode)> results, string continuationToken) items = await GetPageAsync(query, pageSize, continuationToken, cancellationToken);

            return new PageQueryResult<T>(
                countResponse.Resource,
                pageNumber,
                pageSize,
                items.results,
                items.continuationToken);
        }

        private static T GenerateCommonBaseEntity(T value, string operation)
        {
            switch (operation)
            {
                case "Create":
                    value.CreatedAtUtc = DateTime.UtcNow;
                    value.CreatedBy = TokenHelper.AppUser.Id;
                    break;
                case "Update":
                    value.UpdatedAtUtc = DateTime.UtcNow;
                    value.UpdatedBy = TokenHelper.AppUser.Id;
                    break;
                case "Delete":
                    value.DeletedAtUtc = DateTime.UtcNow;
                    value.DeletedBy = TokenHelper.AppUser.Id;
                    break;
                default:
                    break;
            }

            return value;
        }
    }
}
