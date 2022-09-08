using System.Net;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Entities
{
    public class Page<T> : IPage<T> where T : IContainerEntity
    {
        public int Total { get; set; }
        public int PageSize { get; set; }
        public List<(T item, HttpStatusCode statusCode)> Resource { get; set; } = new();
        public string ContinuationToken { get; set; } = default;

        public Page(int total, int pageSize, List<(T item, HttpStatusCode statusCode)> resource, string continuationToken)
        {
            Total = total;
            PageSize = pageSize;
            Resource = resource;
            ContinuationToken = continuationToken;
        }
    }
}
