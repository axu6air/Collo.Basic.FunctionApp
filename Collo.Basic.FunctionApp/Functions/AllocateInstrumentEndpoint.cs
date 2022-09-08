using Collo.Cloud.IAM.Api.Mappers;
using Collo.Cloud.IAM.Api.Routes;
using Collo.Cloud.Services.Libraries.Shared.ApiResponse;
using Collo.Cloud.Services.Libraries.Shared.Authorization;
using Collo.Cloud.Services.Libraries.Shared.Commons;
using Collo.Cloud.Services.Libraries.Shared.Commons.Enums;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Instrument;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Services;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Instrument;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Organizations;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Collo.Cloud.IAM.Api.Functions
{
    public class AllocateInstrumentEndpoint
    {
        private readonly ILogger _logger;
        private readonly IInstrumentService _instrumentService;
        private readonly ServiceFactory _serviceFactory;
        private IAllocatedInstrumentService _allocatedInstrumentService;
        private readonly IOrganizationService _organizationService;

        public AllocateInstrumentEndpoint(IInstrumentService instrumentService, ServiceFactory serviceFactory, IOrganizationService organizationService)
        {
            _logger = LoggerCore.GetLogger<AllocateInstrumentEndpoint>();
            _instrumentService = instrumentService;
            _serviceFactory = serviceFactory;
            _organizationService = organizationService;
        }

        [RouteMapper(RegisteredRoute.ALLOCATE_INSTRUMENT_ADD, RouteOperations.ALLOCATE_INSTRUMENTS_ADD)]
        [Function("AllocateInstruments")]
        public async Task<HttpResponseData> AllocateInstruments([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RegisteredRoute.ALLOCATE_INSTRUMENT_ADD)] HttpRequestData req,
           string organizationId,
           string siteId,
           string gatewayId
       )
        {
            _logger.LogInformation("Instruments Allocated Request Processed!");
            var request = await req.ValidateRequestAsync<List<string>>();
            if (request.IsValid == false)
                return await req.ValidationResult(request.ValidationErrors);

            var instruments = await _instrumentService.GetAsync(request.Value);

            if (instruments.Count == 0)
                return await req.BadRequestResult();

            var alreadyAllocatedInstruments = new List<string>();

            _allocatedInstrumentService = _serviceFactory.GetAllocatedInstrumentService(organizationId, siteId, gatewayId);

            foreach (var instrument in instruments)
            {
                if (instrument.Status.ToLower() == InstrumentStatus.Allocated.ToString().ToLower() && instrument.AllocatedTo != null)
                {
                    alreadyAllocatedInstruments.Add(instrument.Id.ToString());
                    continue;
                }

                var allocateInstrument = CustomMapper.Mapper.Map<AllocatedInstrument>(instrument);
                await _allocatedInstrumentService.AllocateInstrument(allocateInstrument);

                var organization = await _organizationService.GetOrganizationAsync(organizationId);
                var allocatedTo = new AllocatedTo { Id = organization.Id, Name = organization.Name };

                await _instrumentService.AllocateInstrument(instrument.Id, allocatedTo);
            }

            if (alreadyAllocatedInstruments.Count != 0)
                return await req.BadRequestResult(alreadyAllocatedInstruments, string.Empty, message: "These instruments already allocated");

            return await req.OkResult();
        }

        [RouteMapper(RegisteredRoute.ALLOCATE_INSTRUMENTS_GET, RouteOperations.ALLOCATE_INSTRUMENTS_GET)]
        [Function("GetAllocatedInstruments")]
        public async Task<HttpResponseData> GetAllocatedInstrument([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RegisteredRoute.ALLOCATE_INSTRUMENTS_GET)] HttpRequestData req,
            string organizationId,
            string siteId,
            string gatewayId
        )
        {
            _logger.LogInformation("Get Allocated Instruments Request Processed!");

            _allocatedInstrumentService = _serviceFactory.GetAllocatedInstrumentService(organizationId, siteId, gatewayId);
            var instruments = await _allocatedInstrumentService.GetAllocatedInstruments();

            return await req.OkResult(instruments);
        }

        [RouteMapper(RegisteredRoute.ALLOCATE_INSTRUMENTS_DELETE, RouteOperations.ALLOCATE_INSTRUMENTS_DELETE)]
        [Function("DeleteAllocatedInstrument")]
        public async Task<HttpResponseData> DeleteAllocatedInstrument([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = RegisteredRoute.ALLOCATE_INSTRUMENTS_DELETE)] HttpRequestData req,
            string organizationId,
            string siteId,
            string gatewayId,
            string instrumentId
        )
        {
            _logger.LogInformation("Delete Allocated Instrument Request Processed!");

            _allocatedInstrumentService = _serviceFactory.GetAllocatedInstrumentService(organizationId, siteId, gatewayId);
            await _allocatedInstrumentService.DeleteInstrument(instrumentId);

            //ToDo: Make Status Enum, Delete From Allocation Strings using SiteId, GatewayId
            await _instrumentService.UnAllocateInstrument(instrumentId);

            return await req.OkResult();
        }
    }
}
