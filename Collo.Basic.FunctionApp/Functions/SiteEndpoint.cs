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
using System.Net;

namespace Collo.Cloud.IAM.Api.Functions
{
    public class SiteEndpoint
    {
        private readonly ILogger _logger;
        private readonly ServiceFactory _serviceFactory;
        private ISiteService _siteService;
        private readonly IColloPermissionService _colloPermissionService;
        private readonly TreeStructureFactory _treeStructureFactory;

        public SiteEndpoint(
            ILoggerFactory loggerFactory,
            ServiceFactory serviceFactory,
            IColloPermissionService colloPermissionService
        )
        {
            _logger = loggerFactory.CreateLogger<SiteEndpoint>();
            _serviceFactory = serviceFactory;
            _colloPermissionService = colloPermissionService;
        }

        [RouteMapper(RegisteredRoute.SITE_GET_ALL, RouteOperations.SITE_GET_ALL)]
        [Function("GetSites")]
        public async Task<HttpResponseData> GetSites(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route =RegisteredRoute.SITE_GET_ALL)]
            HttpRequestData req,
            string organizationId
        )
        {
            if (string.IsNullOrEmpty(organizationId))
                return await req.OkResult(data: null, status: "Bad Request", message: "Organization Id cannot be empty", statusCode: HttpStatusCode.BadRequest);

            var result = req.GetPermittedObjects();

            _siteService = _serviceFactory.GetSiteService(organizationId);
            var sites = await _siteService.GetSitesWithPermissionsAsync(result.permittedChildrenIds, result.isAnyPermitted);
            var data = CustomMapper.Mapper.Map<List<SiteResponseModel>>(sites);

            return await req.OkResult(data);
        }

        [RouteMapper(RegisteredRoute.SITE_GET, RouteOperations.SITE_GET)]
        [Function("GetSite")]
        public async Task<HttpResponseData> GetSite(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RegisteredRoute.SITE_GET)]
            HttpRequestData req,
            string organizationId,
            string siteId
        )
        {
            if (string.IsNullOrEmpty(organizationId))
                return await req.OkResult(data: null, status: "Bad Request", message: "Organization Id cannot be empty", statusCode: HttpStatusCode.BadRequest);
            if (string.IsNullOrEmpty(siteId))
                return await req.OkResult(data: null, status: "Bad Request", message: "Site Id cannot be empty", statusCode: HttpStatusCode.BadRequest);

            _siteService = _serviceFactory.GetSiteService(organizationId);
            var site = await _siteService.GetSiteAsync(siteId);
            var data = CustomMapper.Mapper.Map<SiteResponseModel>(site);

            return await req.OkResult(data);
        }

        [RouteMapper(RegisteredRoute.SITE_CREATE, RouteOperations.SITE_CREATE)]
        [Function("CreateSite")]
        public async Task<HttpResponseData> CreateSite(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RegisteredRoute.SITE_CREATE)]
            HttpRequestData req,
            string organizationId
        )
        {
            try
            {
                var request = await req.ValidateRequestAsync<CreateSiteRequestModel>();
                if (!request.IsValid)
                    return await req.ValidationResult(request.ValidationErrors);

                _siteService = _serviceFactory.GetSiteService(organizationId);

                if (_siteService.ExistsByName(request.Value.Name))
                    return await req.BadRequestResult(
                        data: "Site with the same name already exists!",
                        status: "ValidationError",
                        message: "Validation Failed"
                    );

                var site = CustomMapper.Mapper.Map<Site>(request.Value);
                await _siteService.CreateSiteAsync(site);

                return await req.OkResult(CustomMapper.Mapper.Map<SiteResponseModel>(site));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Site exception ex:{ex.Message}");
                throw;
            }
        }

        [RouteMapper(RegisteredRoute.SITE_DELETE, RouteOperations.SITE_DELETE)]
        [Function("DeleteSite")]
        public async Task<HttpResponseData> DeleteSite(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = RegisteredRoute.SITE_DELETE)]
            HttpRequestData req,
            string organizationId,
            string siteId
        )
        {
            try
            {
                //ToDo: Soft Delete, Optimize Soft Delete
                _siteService = _serviceFactory.GetSiteService(organizationId);
                await _siteService.DeleteSiteAsync(siteId);
                return await req.OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Site exception ex:{ex.Message}");
                throw;
            }
        }

        [RouteMapper(RegisteredRoute.SITE_UPDATE, RouteOperations.SITE_UPDATE)]
        [Function("UpdateSite")]
        public async Task<HttpResponseData> UpdateSite([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = RegisteredRoute.SITE_UPDATE)]
            HttpRequestData req,
            string organizationId,
            string siteId)
        {
            _logger.LogInformation("Update Site Request Processed!");

            var request = await req.ValidateRequestAsync<UpdateSiteRequestModel>();

            if (!request.IsValid)
                return await req.ValidationResult(request.ValidationErrors);

            _siteService = _serviceFactory.GetSiteService(organizationId);

            var site = await _siteService.GetSiteAsync(siteId);
            CustomMapper.Mapper.Map(request.Value, site);

            await _siteService.UpdateSiteAsync(site);

            return await req.OkResult(CustomMapper.Mapper.Map<UpdateSiteResponseModel>(site));
        }
    }
}
