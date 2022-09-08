using Collo.Cloud.IAM.Api.Mappers;
using Collo.Cloud.IAM.Api.Models.Assignment;
using Collo.Cloud.IAM.Api.Routes;
using Collo.Cloud.Services.Libraries.Shared.ApiResponse;
using Collo.Cloud.Services.Libraries.Shared.Authorization;
using Collo.Cloud.Services.Libraries.Shared.Helpers;
using Collo.Cloud.Services.Libraries.Shared.Permission.Interfaces;
using Collo.Cloud.Services.Libraries.Shared.Permission.Models;
using Collo.Cloud.Services.Libraries.Shared.Validations;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Collo.Cloud.IAM.Api.Functions
{
    public class PermissionEndpoint
    {
        private readonly ILogger _logger;
        private readonly IColloPermissionService _colloPermissionService;

        public PermissionEndpoint(ILoggerFactory loggerFactory, IColloPermissionService colloPermissionService)
        {
            _logger = loggerFactory.CreateLogger<PermissionEndpoint>();
            _colloPermissionService = colloPermissionService;
        }

        #region GetPermissionList
        //[RouteMapper(RegisteredRoute.PERMISSION_GET_ALL, RouteOperations.PERMISSION_GET_PERMISSION_LIST)]
        //[Function("GetPermissionList")]
        //public async Task<HttpResponseData> GetPermissionListAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "iam/service/organizations/users/permissions")] HttpRequestData req)
        //{
        //    _logger.LogInformation("Get Assignment Request Processed");

        //    var data = PermissionEnumToListHelper.Convert();

        //    return await req.OkResult(data);
        //}
        #endregion

        //[RouteMapper(RegisteredRoute.PERMISSION_CREATE_ASSIGNMENT, RouteOperations.PERMISSION_CREATE_ASSIGNMENT)]
        //[Function("CreateAssignment")]
        //public async Task<HttpResponseData> CreateAssignment([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RegisteredRoute.PERMISSION_CREATE_ASSIGNMENT)] HttpRequestData req)
        //{
        //    _logger.LogInformation("Create Assignment Request Processed");
        //    var request = await req.ValidateRequestAsync<CreateAssignmentModel>();
        //    if (request.IsValid == false)
        //        return await req.ValidationResult(request.ValidationErrors);

        //    var response = await _colloPermissionService.AddAsync(new PermissionRequest(), request.Value.UserId, request.Value.OrganizationId);

        //    if (response != null)
        //        return await req.OkResult();
        //    else
        //        return await req.BadRequestResult();
        //}

        [RouteMapper(RegisteredRoute.PERMISSION_ADD_ASSIGNMENT, RouteOperations.PERMISSION_ADD_ASSIGNMENT)]
        [Function("AddAssignments")]
        public async Task<HttpResponseData> AddAssignments([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = RegisteredRoute.PERMISSION_ADD_ASSIGNMENT)] HttpRequestData req, string userId, string organizationId)
        {
            _logger.LogInformation("Add Assignment Request Processed");
            var request = await req.ValidateRequestAsync<AssignmentRequestModel>();
            if (request.IsValid == false)
                return await req.ValidationResult(request.ValidationErrors);

            var data = CustomMapper.Mapper.Map<AssignmentModel>(request.Value);

            var assignment = AssignmentMaker.GetAssignmentString(data);

            if (assignment == null)
                return await req.BadRequestResult();

            var checkAssignment = AssignmentValidation.CheckAssignment(assignment, data);

            if (!checkAssignment)
                return await req.BadRequestResult();

            var response = await _colloPermissionService.AddAssignmentsAsync(assignment, organizationId, userId);

            if (response == null)
                return await req.BadRequestResult();

            return await req.OkResult(response);
        }

        #region AddPermission
        //[Function("AddPermission")]
        //public async Task<HttpResponseData> CreateRolePermissionAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RegisteredRoute.PERMISSION_CREATE_ROLE_PERMISSION)] HttpRequestData req, string userId, string organizationId)
        //{
        //    _logger.LogInformation("Create Permission Request Processed");
        //    var request = await req.ValidateRequestAsync<List<RolePermissionModel>>();
        //    if (request.IsValid == false)
        //        return await req.ValidationResult(request.ValidationErrors);

        //    var data = CustomMapper.Mapper.Map<List<PermissionModel>>(request.Value);

        //    var permissions = AssignmentMaker.GetPermission(data);

        //    if (permissions.Count == 0)
        //        return await req.BadRequestResult();
        //    var response = await _colloPermissionService.AddPermissionAsync(permissions, organizationId, userId);

        //    if (response == null)
        //        return await req.BadRequestResult();

        //    return await req.OkResult();

        //}
        #endregion

        #region GetAssignment

        //[RouteMapper(RegisteredRoute.PERMISSION_GET_ASSIGNMENT, RouteOperations.PERMISSION_GET_ASSIGNMENT)]
        //[Function("GetAssignment")]
        //public async Task<HttpResponseData> GetPermissionAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "iam/service/organizations/{organizationId}/users/{userId}/assignment")] HttpRequestData req, string userId, string organizationId)
        //{
        //    _logger.LogInformation("Get Assignment Request Processed");

        //    var response = await _colloPermissionService.GetAsync(userId, organizationId);

        //    return await req.OkResult(CustomMapper.Mapper.Map<CreateUpdateAssignmentResponseModel>(response));
        //}
        #endregion

        //[RouteMapper(RegisteredRoute.PERMISSION_ADD_ROLES, RouteOperations.PERMISSION_ADD_ROLES)]
        //[Function("AddPermission")]
        //public async Task<HttpResponseData> AddPermission([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = RegisteredRoute.PERMISSION_ADD_ROLES)] HttpRequestData req, string userId, string organizationId)
        //{
        //    _logger.LogInformation("Update Roles Request Processed");

        //    var request = await req.ValidateRequestAsync<List<RolePermissionModel>>();
        //    if (request.IsValid == false)
        //        return await req.ValidationResult(request.ValidationErrors);

        //    var data = CustomMapper.Mapper.Map<List<PermissionModel>>(request.Value);

        //    var permission = AssignmentMaker.GetRolePermission(data);

        //    if (permission.Count == 0)
        //        return await req.BadRequestResult();

        //    var response = await _colloPermissionService.AddRolesAsync(organizationId, userId, permission);

        //    if (response == null)
        //        return await req.BadRequestResult();

        //    return await req.OkResult(response);
        //}

        [RouteMapper(RegisteredRoute.PERMISSION_DELETE_ASSIGNMENT, RouteOperations.PERMISSION_DELETE_ASSIGNMENT)]
        [Function("DeleteAssignment")]
        public async Task<HttpResponseData> DeleteAssignment([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = RegisteredRoute.PERMISSION_DELETE_ASSIGNMENT)] HttpRequestData req, string userId, string organizationId)
        {
            _logger.LogInformation("Create Assignment Request Processed");
            var request = await req.ValidateRequestAsync<AssignmentRequestModel>();
            if (request.IsValid == false)
                return await req.ValidationResult(request.ValidationErrors);

            var data = CustomMapper.Mapper.Map<AssignmentModel>(request.Value);

            var assignment = AssignmentMaker.GetAssignmentString(data);

            if (assignment == null)
                return await req.BadRequestResult();

            var checkAssignment = AssignmentValidation.CheckAssignment(assignment, data);

            if (!checkAssignment)
                return await req.BadRequestResult();

            var response = await _colloPermissionService.DeleteAssignment(assignment, organizationId, userId);

            if (!response)
                return await req.BadRequestResult();

            return await req.OkResult();

        }

        #region UpdateAssignment
        //[RouteMapper(RegisteredRoute.PERMISSION_UPDATE_ASSIGNMENT, RouteOperations.PERMISSION_UPDATE_ASSIGNMENT)]
        //[Function("UpdateAssignment")]
        //public async Task<HttpResponseData> UpdateAssignment([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "iam/service/organizations/{organizationId}/users/{userId}/assignments")] HttpRequestData req, string userId, string organizationId)
        //{
        //    _logger.LogInformation("Update Assignments Request Processed");

        //    var request = await req.ValidateRequestAsync<UpdateAssignmentRequestModel>();
        //    if (request.IsValid == false)
        //        return await req.ValidationResult(request.ValidationErrors);

        //    var response = await _colloPermissionService.UpdateAssignments(userId, organizationId, request.Value.Assignments);

        //    return await req.OkResult(CustomMapper.Mapper.Map<CreateUpdateAssignmentResponseModel>(response));
        //}
        #endregion
    }
}
