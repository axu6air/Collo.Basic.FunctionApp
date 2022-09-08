using Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Entities;
using System.Net;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Response
{
    public class BaseRepositoryResponse<T> where T : ContainerEntity
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public object Value { get; set; }

    }
}
