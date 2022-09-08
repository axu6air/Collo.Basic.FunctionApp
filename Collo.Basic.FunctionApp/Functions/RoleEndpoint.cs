using Collo.Cloud.IAM.Api.Mappers;
using Collo.Cloud.IAM.Api.Models.Request;
using Collo.Cloud.IAM.Api.Models.Response;
using Collo.Cloud.IAM.Api.Routes;
using Collo.Cloud.Services.Libraries.Shared.ApiResponse;
using Collo.Cloud.Services.Libraries.Shared.Authorization;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Services;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Organizations;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Collo.Cloud.IAM.Api.Functions
{
    public class RoleEndpoint
    {
        private readonly ILogger _logger;
        private IRoleService _roleService;
        private readonly ServiceFactory _serviceFactory;

        public RoleEndpoint(ILoggerFactory loggerFactory, ServiceFactory serviceFactory)
        {
            _logger = loggerFactory.CreateLogger<RoleEndpoint>();
            _serviceFactory = serviceFactory;

        }

        [RouteMapper(RegisteredRoute.ROLE_GET_ALL, RouteOperations.ROLE_GET_ALL)]
        [Function("GetRoles")]
        public async Task<HttpResponseData> GetRoles([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RegisteredRoute.ROLE_GET_ALL)]
        HttpRequestData req, string organizationId)
        {
            _logger.LogInformation("Get Roles Request Processed!");

            _roleService = _serviceFactory.GetRoleService(organizationId);
            var roles = await _roleService.GetRoles(organizationId);

            return await req.OkResult(CustomMapper.Mapper.Map<List<RoleResponseModel>>(roles));
        }

        [RouteMapper(RegisteredRoute.ROLE_CREATE, RouteOperations.ROLE_CREATE)]
        [Function("CreateRole")]
        public async Task<HttpResponseData> CreateRoles([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RegisteredRoute.ROLE_CREATE)]
        HttpRequestData req, string organizationId)
        {
            _logger.LogInformation("Create Role Request Processed!");

            var request = await req.ValidateRequestAsync<CreateRoleRequestModel>();

            if (request.IsValid == false)
                return await req.ValidationResult(request.ValidationErrors);

            var data = CustomMapper.Mapper.Map<Role>(request.Value);

            ////Append Child Document (Roles) in 'Organization'

            _roleService = _serviceFactory.GetRoleService(organizationId);

            var roles = await _roleService.CreateRole(organizationId, data);

            if (roles == null)
                return await req.BadRequestResult();

            return await req.OkResult(CustomMapper.Mapper.Map<List<RoleResponseModel>>(roles));
        }

        [RouteMapper(RegisteredRoute.ROLE_DELETE, RouteOperations.ROLE_DELETE)]
        [Function("DeleteRole")]
        public async Task<HttpResponseData> DeleteRole([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = RegisteredRoute.ROLE_DELETE)]
        HttpRequestData req, string organizationId, string roleId)
        {
            _logger.LogInformation("Delete Role Request Processed!");

            //ToDo: Optimize Soft Delete
            _roleService = _serviceFactory.GetRoleService(organizationId);

            var response = await _roleService.DeleteRole(organizationId, roleId);
            if (response == false)
                return await req.BadRequestResult();

            return await req.OkResult();
        }

        [RouteMapper(RegisteredRoute.ROLE_GET, RouteOperations.ROLE_GET)]
        [Function("GetRoleById")]
        public async Task<HttpResponseData> GetRoleById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RegisteredRoute.ROLE_GET)]
        HttpRequestData req, string organizationId, string roleId)
        {
            _logger.LogInformation("Get Role By Id Request Processed!");

            _roleService = _serviceFactory.GetRoleService(organizationId);
            var role = await _roleService.GetRole(organizationId, roleId);

            if (role == null)
                return await req.BadRequestResult();
            return await req.OkResult(CustomMapper.Mapper.Map<GetRoleResponseModel>(role));
        }

        [RouteMapper(RegisteredRoute.ROLE_UPDATE, RouteOperations.ROLE_UPDATE)]
        [Function("UpdateRole")]
        public async Task<HttpResponseData> UpdateRole([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = RegisteredRoute.ROLE_UPDATE)]
        HttpRequestData req, string organizationId, string roleId)
        {
            _logger.LogInformation("Update Role Request Processed!");

            var request = await req.ValidateRequestAsync<UpdateRoleRequestModel>();

            if (!request.IsValid)
                return await req.ValidationResult(request.ValidationErrors);

            var data = CustomMapper.Mapper.Map<Role>(request.Value);

            _roleService = _serviceFactory.GetRoleService(organizationId);
            var roles = await _roleService.UpdateRole(organizationId, roleId, data);
            if (roles == null)
                return await req.BadRequestResult();

            return await req.OkResult(CustomMapper.Mapper.Map<List<UpdateRoleResponseModel>>(roles));
        }
    }
}
