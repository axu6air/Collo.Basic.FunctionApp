using Collo.Cloud.IAM.Api.Mappers;
using Collo.Cloud.IAM.Api.Models.Organization;
using Collo.Cloud.Services.Libraries.Shared.ApiResponse;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Organizations;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Collo.Basic.FunctionApp
{
    public class Function1
    {
        private readonly ILogger _logger;
        private readonly IOrganizationService _organizationService;

        public Function1(ILoggerFactory loggerFactory, IOrganizationService organizationService)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
            _organizationService = organizationService;
        }

        [Function("Function1")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            var organizations = await _organizationService.GetOrganizationAsync();

            return await req.OkResult(CustomMapper.Mapper.Map<List<GetOrgResponseModel>>(organizations));
        }
    }
}
