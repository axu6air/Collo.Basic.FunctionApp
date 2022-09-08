using System.Net;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Entities
{
    public interface IPageQueryResult<T> where T : IContainerEntity
    {
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<(T item, HttpStatusCode statusCode)> Resource { get; set; }
        string ContinuationToken { get; set; }
    }
}
