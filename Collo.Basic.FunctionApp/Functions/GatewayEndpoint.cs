using Collo.Cloud.IAM.Api.Mappers;
using Collo.Cloud.IAM.Api.Models.Request;
using Collo.Cloud.IAM.Api.Models.Response;
using Collo.Cloud.IAM.Api.Routes;
using Collo.Cloud.Services.Libraries.Shared.ApiResponse;
using Collo.Cloud.Services.Libraries.Shared.Authorization;
using Collo.Cloud.Services.Libraries.Shared.Extensions;
using Collo.Cloud.Services.Libraries.Shared.Permission.Interfaces;
using Collo.Cloud.Services.Libraries.Shared.Permission.PermissionTree;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Services;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Organizations;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Collo.Cloud.IAM.Api.Functions
{
    public class GatewayEndpoint
    {
        private readonly ILogger _logger;
        private readonly ServiceFactory _serviceFactory;
        private IGatewayService _gatewayService;
        private readonly IColloPermissionService _colloPermissionService;
        private readonly TreeStructureFactory _treeStructureFactory;
        private readonly ITreeStructure _treeStructure;

        public GatewayEndpoint(
            ILoggerFactory loggerFactory,
            ServiceFactory serviceFactory,
            IColloPermissionService colloPermissionService,
            TreeStructureFactory treeStructureFactory
        )
        {
            _logger = loggerFactory.CreateLogger<GatewayEndpoint>();
            _serviceFactory = serviceFactory;
            _colloPermissionService = colloPermissionService;
            _treeStructureFactory = treeStructureFactory;
        }

        [RouteMapper(RegisteredRoute.GATEWAY_CREATE, RouteOperations.GATEWAY_CREATE)]
        [Function("CreateGateway")]
        public async Task<HttpResponseData> CreateGateway(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RegisteredRoute.GATEWAY_CREATE)]
            HttpRequestData req,
            string organizationId,
            string siteId
        )
        {
            _logger.LogInformation("Create Gateway Request Processed!");

            try
            {
                var request = await req.ValidateRequestAsync<CreateGatewayRequestModel>();
                if (!request.IsValid)
                    return await req.ValidationResult(request.ValidationErrors);

                _gatewayService = _serviceFactory.GetGatewayService(organizationId, siteId);

                if (_gatewayService.ExistsByName(request.Value.Name))
                    return await req.BadRequestResult(
                        data: "Gateway with the same name already exists!",
                        status: "ValidationError",
                        message: "Validation Failed"
                    );

                var gateway = CustomMapper.Mapper.Map<Gateway>(request.Value);
                await _gatewayService.CreateGatewayAsync(gateway);

                return await req.OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Gateway exception ex:{ex.Message}");
                throw;
            }
        }

        [RouteMapper(RegisteredRoute.GATEWAY_GET_ALL, RouteOperations.GATEWAY_GET_ALL)]
        [Function("GetGateways")]
        public async Task<HttpResponseData> GetGateways(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RegisteredRoute.GATEWAY_GET_ALL)]
            HttpRequestData req,
            string organizationId,
            string siteId
        )
        {
            _logger.LogInformation("Get Gateways Request Processed!");

            _gatewayService = _serviceFactory.GetGatewayService(organizationId, siteId);
            var result = req.GetPermittedObjects();

            var gateways = await _gatewayService.GetGatewaysWithPermissionAsync(result.permittedChildrenIds, result.isAnyPermitted);

            var data = CustomMapper.Mapper.Map<List<GatewayResponseModel>>(gateways);

            return await req.OkResult(data);
        }

        [RouteMapper(RegisteredRoute.GATEWAY_GET, RouteOperations.GATEWAY_GET)]
        [Function("GetGateway")]
        public async Task<HttpResponseData> GetGateway(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RegisteredRoute.GATEWAY_GET)]
            HttpRequestData req,
            string organizationId,
            string siteId,
            string gatewayId
        )
        {
            _logger.LogInformation("Get Gateway Request Processed!");

            _gatewayService = _serviceFactory.GetGatewayService(organizationId, siteId);
            var gateway = await _gatewayService.GetGatewayAsync(gatewayId);

            var data = CustomMapper.Mapper.Map<GatewayResponseModel>(gateway);

            return await req.OkResult(data);
        }

        [RouteMapper(RegisteredRoute.GATEWAY_DELETE, RouteOperations.GATEWAY_DELETE)]
        [Function("DeleteGateway")]
        public async Task<HttpResponseData> DeleteGateway(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = RegisteredRoute.GATEWAY_DELETE)]
            HttpRequestData req,
            string organizationId,
            string siteId,
            string gatewayId
        )
        {
            _logger.LogInformation("Delete Gateway Request Processed!");

            _gatewayService = _serviceFactory.GetGatewayService(organizationId, siteId);
            await _gatewayService.DeleteGatewayAsync(gatewayId);

            return await req.OkResult();
        }

        [RouteMapper(RegisteredRoute.GATEWAY_UPDATE, RouteOperations.GATEWAY_UPDATE)]
        [Function("UpdateGateway")]
        public async Task<HttpResponseData> UpdateGateway([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route =  RegisteredRoute.GATEWAY_UPDATE)]
            HttpRequestData req,
            string organizationId,
            string siteId,
            string gatewayId)
        {
            _logger.LogInformation("Update Gateway Request Processed!");

            try
            {
                var request = await req.ValidateRequestAsync<UpdateGatewayRequestModel>();

                if (!request.IsValid)
                    return await req.ValidationResult(request.ValidationErrors);

                _gatewayService = _serviceFactory.GetGatewayService(organizationId, siteId);
                var gateway = await _gatewayService.GetGatewayAsync(gatewayId);
                CustomMapper.Mapper.Map(request.Value, gateway);

                await _gatewayService.UpdateGatewayAsync(gateway);

                return await req.OkResult(CustomMapper.Mapper.Map<UpdateGatewayResponseModel>(gateway));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Gateway exception ex:{ex.Message}");
                throw;
            }
        }
    }
}
