using Collo.Cloud.IAM.Api.Mappers;
using Collo.Cloud.IAM.Api.Models.Organization;
using Collo.Cloud.IAM.Api.Routes;
using Collo.Cloud.Services.Libraries.Shared.ApiResponse;
using Collo.Cloud.Services.Libraries.Shared.Authorization;
using Collo.Cloud.Services.Libraries.Shared.Helpers;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Assignment;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Repositories;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Services;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Organizations;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Collo.Cloud.IAM.Api.Functions
{
    public class OrganizationEndpoint
    {
        private readonly ILogger _logger;
        private readonly IOrganizationService _organizationService;
        private readonly IRepository<Assignment> _assignmentRepository;
        private readonly ServiceFactory _serviceFactory;

        public OrganizationEndpoint(ILoggerFactory loggerFactory, IOrganizationService organizationService, IRepository<Assignment> assignmentRepository, ServiceFactory serviceFactory)
        {
            _logger = loggerFactory.CreateLogger<OrganizationEndpoint>();
            _organizationService = organizationService;
            _assignmentRepository = assignmentRepository;
            _serviceFactory = serviceFactory;
        }

        [RouteMapper(RegisteredRoute.ORGANIZATION_UPDATE, RouteOperations.ORGANIZATION_UPDATE)]
        [Function("UpdateOrganization")]
        public async Task<HttpResponseData> Update([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = RegisteredRoute.ORGANIZATION_UPDATE)] HttpRequestData req, string organizationId)
        {
            _logger.LogInformation("Organization Update Request Processed!");

            var request = await req.ValidateRequestAsync<UpdateOrgRequestModel>();
            if (request.IsValid == false)
                return await req.ValidationResult(request.ValidationErrors);

            var organzation = await _organizationService.GetOrganizationAsync(organizationId);
            CustomMapper.Mapper.Map(request.Value, organzation);
            await _organizationService.Update(organzation);

            return await req.OkResult(CustomMapper.Mapper.Map<UpdateOrgResponseModel>(organzation));
        }

        [RouteMapper(RegisteredRoute.ORGANIATION_CREATE, RouteOperations.ORGANIATION_CREATE)]
        [Function("CreateOrganization")]
        public async Task<HttpResponseData> Create([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RegisteredRoute.ORGANIATION_CREATE)] HttpRequestData req)
        {
            var request = await req.ValidateRequestAsync<CreateOrgRequestModel>();

            if (request.IsValid == false)
                return await req.ValidationResult(request.ValidationErrors);

            if (await _organizationService.ExistsByName(request.Value.Name))
                return await req.BadRequestResult(
                        data: "Organization with the same name already exists!",
                        status: "ValidationError",
                        message: "Validation Failed"
                    );

            var organization = CustomMapper.Mapper.Map<Organization>(request.Value);

            organization.Settings.PubSubsConnectionStringKV = organization.Id; // Temporarily
            organization.Roles = GetStaticRoles();

            var createdOrganization = await _organizationService.CreateAsync(organization);

            return await req.OkResult(CustomMapper.Mapper.Map<CreateOrgResponeModel>(createdOrganization));
        }

        [RouteMapper(RegisteredRoute.ORGANIZATION_DELETE, RouteOperations.ORGANIZATION_DELETE)]
        [Function("DeleteOrganization")]
        public async Task<HttpResponseData> Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = RegisteredRoute.ORGANIZATION_DELETE)] HttpRequestData req, string organizationId)
        {
            _logger.LogInformation("Organization Delete Request Processed!");
            //Soft Delete
            var response = await _organizationService.DeleteOrganization(organizationId);

            if (response == false)
                return await req.BadRequestResult();

            return await req.OkResult();
        }

        [RouteMapper(RegisteredRoute.ORGANIZATION_GET_ALL, RouteOperations.ORGANIZATION_GET_ALL)]
        [Function("GetAllOrganization")]
        public async Task<HttpResponseData> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RegisteredRoute.ORGANIZATION_GET_ALL)] HttpRequestData req)
        {
            _logger.LogInformation("Organization Get All Request Processed!");

            var organizations = await _organizationService.GetOrganizationAsync();

            return await req.OkResult(CustomMapper.Mapper.Map<List<GetOrgResponseModel>>(organizations));
        }

        [RouteMapper(RegisteredRoute.ORGANIZATION_GET, RouteOperations.ORGANIZATION_GET)]
        [Function("GetOrganizationById")]
        public async Task<HttpResponseData> GetById([HttpTrigger(AuthorizationLevel.Anonymous, methods: "get",
            Route = RegisteredRoute.ORGANIZATION_GET)] HttpRequestData req, string organizationId)
        {
            _logger.LogInformation("Organization Get By Id Request Processed!");

            var organization = await _organizationService.GetOrganizationAsync(organizationId);

            if (organization == null)
                return await req.BadRequestResult();

            return await req.OkResult(CustomMapper.Mapper.Map<GetOrgResponseModel>(organization));
        }

        private static List<Role> GetStaticRoles()
        {
            Role adminRole = new()
            {
                Id = IdHelper.NewNanoId(),
                IsDeleted = false,
                Name = "admin",
                DisplayName = "Admin",
                Permissions = new List<char> { 'c', 'r', 'u', 'd', 'l' }
            };

            Role userRole = new()
            {
                Id = IdHelper.NewNanoId(),
                IsDeleted = false,
                Name = "user",
                DisplayName = "User",
                Permissions = new List<char> { 'r', 'l' }
            };

            List<Role> roles = new() { adminRole, userRole };

            return roles;
        }
    }
}
