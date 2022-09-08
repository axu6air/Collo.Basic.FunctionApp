using Collo.Cloud.IAM.Api.Mappers;
using Collo.Cloud.IAM.Api.Models.Request;
using Collo.Cloud.IAM.Api.Models.Response;
using Collo.Cloud.IAM.Api.Routes;
using Collo.Cloud.Services.Libraries.Shared.ApiResponse;
using Collo.Cloud.Services.Libraries.Shared.Authorization;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Instrument;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Instrument;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Collo.Cloud.IAM.Api.Functions
{
    public class InstrumentEndpoint
    {
        private readonly ILogger _logger;
        private readonly IInstrumentService _instrumentService;

        public InstrumentEndpoint(
            ILoggerFactory loggerFactory,
            IInstrumentService instrumentService
        )
        {
            _logger = loggerFactory.CreateLogger<InstrumentEndpoint>();
            _instrumentService = instrumentService;
        }

        [RouteMapper(RegisteredRoute.INSTRUMENT_ADD, RouteOperations.INSTRUMENT_ADD)]
        [Function("AddInstrument")]
        public async Task<HttpResponseData> AddInstrument([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RegisteredRoute.INSTRUMENT_ADD)] HttpRequestData req)
        {
            _logger.LogInformation("Instrument Add Request Processed!");
            var request = await req.ValidateRequestAsync<CreateInstrumentRequestModel>();
            if (request.IsValid == false)
                return await req.ValidationResult(request.ValidationErrors);

            var data = CustomMapper.Mapper.Map<Instrument>(request.Value);
            var response = await _instrumentService.CreateAsync(data);

            return await req.OkResult(CustomMapper.Mapper.Map<InstrumentResponseModel>(response));
        }

        [RouteMapper(RegisteredRoute.INSTRUMENT_UPDATE, RouteOperations.INSTRUMENT_UPDATE)]
        [Function("UpdateInstrument")]
        public async Task<HttpResponseData> UpdateInstrument([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = RegisteredRoute.INSTRUMENT_UPDATE)] HttpRequestData req, string instrumentId)
        {
            _logger.LogInformation("Instrument Update Request Processed!");
            var request = await req.ValidateRequestAsync<UpdateInstrumentRequestModel>();
            if (request.IsValid == false)
                return await req.ValidationResult(request.ValidationErrors);

            var instrument = await _instrumentService.GetAsync(instrumentId);
            CustomMapper.Mapper.Map(request.Value, instrument);
            await _instrumentService.UpdateAsync(instrument);

            return await req.OkResult(CustomMapper.Mapper.Map<InstrumentResponseModel>(instrument));
        }

        [RouteMapper(RegisteredRoute.INSTRUMENT_GET_ALL, RouteOperations.INSTRUMENT_GET_ALL)]
        [Function("GetInstruments")]
        public async Task<HttpResponseData> GetInstruments([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RegisteredRoute.INSTRUMENT_GET_ALL)] HttpRequestData req)
        {
            _logger.LogInformation("Instrument Get Request Processed!");

            var response = await _instrumentService.GetAsync();

            return await req.OkResult(CustomMapper.Mapper.Map<List<InstrumentResponseModel>>(response));
        }

        [RouteMapper(RegisteredRoute.INSTRUMENT_GET_UNALLOCATEDINSTRUMENT, RouteOperations.INSTRUMENT_GET_UNALLOCATEDINSTRUMENT)]
        [Function("GetUnallocatedInstruments")]
        public async Task<HttpResponseData> GetUnallocatedInstruments([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RegisteredRoute.INSTRUMENT_GET_UNALLOCATEDINSTRUMENT)] HttpRequestData req)
        {
            _logger.LogInformation("Unallocated instrument Get Request Processed!");

            var response = await _instrumentService.GetUnallocatedInstrumentsAsync();

            return await req.OkResult(CustomMapper.Mapper.Map<List<InstrumentResponseModel>>(response));
        }

        [RouteMapper(RegisteredRoute.INSTRUMENT_DELETE, RouteOperations.INSTRUMENT_DELETE)]
        [Function("DeleteInstrument")]
        public async Task<HttpResponseData> DeleteInstrument([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = RegisteredRoute.INSTRUMENT_DELETE)] HttpRequestData req, string instrumentId)
        {
            _logger.LogInformation("Instrument Delete Request Processed!");

            //ToDo: Delete from Allocations as well
            var instrument = await _instrumentService.DeleteAsync(instrumentId);

            return await req.OkResult(CustomMapper.Mapper.Map<InstrumentResponseModel>(instrument));
        }

        [RouteMapper(RegisteredRoute.INSTRUMENT_GET, RouteOperations.INSTRUMENT_GETBYID)]
        [Function("GetInstrumentById")]
        public async Task<HttpResponseData> GetInstrumentById([HttpTrigger(AuthorizationLevel.Anonymous,
            "get", Route = RegisteredRoute.INSTRUMENT_GET)] HttpRequestData req, string instrumentId)
        {
            _logger.LogInformation("Get Instrument By Id Request Processed");

            var instrument = await _instrumentService.GetAsync(instrumentId);

            return await req.OkResult(CustomMapper.Mapper.Map<InstrumentResponseModel>(instrument));
        }
    }
}
