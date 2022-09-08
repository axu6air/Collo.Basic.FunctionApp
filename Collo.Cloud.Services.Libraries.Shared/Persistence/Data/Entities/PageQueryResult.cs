using System.Net;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Entities
{
    public class PageQueryResult<T> : IPageQueryResult<T> where T : IContainerEntity
    {
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<(T item, HttpStatusCode statusCode)> Resource { get; set; } = new();
        public string ContinuationToken { get; set; } = default;

        public PageQueryResult(int totalPages, int pageNumber, int pageSize, List<(T item, HttpStatusCode statusCode)> resource, string continuationToken)
        {
            TotalPages = totalPages;
            PageNumber = pageNumber;
            PageSize = pageSize;
            Resource = resource;
            ContinuationToken = continuationToken;
        }
    }
}
