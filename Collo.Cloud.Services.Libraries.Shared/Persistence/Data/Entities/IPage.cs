using System.Net;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Entities
{
    public interface IPage<T> where T : IContainerEntity
    {
        public int Total { get; set; }
        public int PageSize { get; set; }
        public List<(T item, HttpStatusCode statusCode)> Resource { get; set; }

    }
}
