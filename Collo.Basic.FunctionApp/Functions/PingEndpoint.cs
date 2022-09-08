using Collo.Cloud.Services.Libraries.Shared.ApiResponse;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Collo.Cloud.IAM.Api.Functions
{
    public class PingEndpoint
    {
        private readonly ILogger _logger;

        public PingEndpoint(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<PingEndpoint>();
        }

        [Function("PingEndpoint")]
        public async Task<HttpResponseData> PingEndpoint1([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            return await req.OkResult();
        }

        [Function("PingEndpoint2")]
        public async Task<HttpResponseData> PingEndpoint2([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            return await req.OkResult();
        }

        [Function("PingEndpoint3")]
        public async Task<HttpResponseData> PingEndpoint3([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            return await req.OkResult();
        }
    }
}
