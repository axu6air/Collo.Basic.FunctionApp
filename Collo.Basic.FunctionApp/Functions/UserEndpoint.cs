using Collo.Cloud.IAM.Api.Mappers;
using Collo.Cloud.IAM.Api.Models.User;
using Collo.Cloud.IAM.Api.Routes;
using Collo.Cloud.Services.Libraries.Shared.ApiResponse;
using Collo.Cloud.Services.Libraries.Shared.Authorization;
using Collo.Cloud.Services.Libraries.Shared.B2C;
using Collo.Cloud.Services.Libraries.Shared.B2C.Configurations;
using Collo.Cloud.Services.Libraries.Shared.Helpers;
using Collo.Cloud.Services.Libraries.Shared.Permission.Interfaces;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Services;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Organizations;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using System.Net;
using CosmosUser = Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization.User;

namespace Collo.Cloud.IAM.Api.Functions
{
    public class UserEndpoint
    {
        private readonly ILogger _logger;
        private readonly ServiceFactory _serviceFactory;
        private readonly IB2cUserService _b2cUserService;
        private readonly IB2cUserServiceConfiguration _b2CUserServiceConfig;
        private readonly IColloPermissionService _colloPermissionService;
        private readonly IOrganizationService _organizationService;
        private IUserService _userService;

        public UserEndpoint(ILoggerFactory loggerFactory, ServiceFactory serviceFactory, IB2cUserService b2CUserService, IB2cUserServiceConfiguration b2CUserServiceConfig, IColloPermissionService colloPermissionService, IOrganizationService organizationService)
        {
            _logger = loggerFactory.CreateLogger<UserEndpoint>();
            _serviceFactory = serviceFactory;
            _b2cUserService = b2CUserService;
            _b2CUserServiceConfig = b2CUserServiceConfig;
            _colloPermissionService = colloPermissionService;
            _organizationService = organizationService;
        }

        [RouteMapper(RegisteredRoute.USERS_GET_ALL, RouteOperations.USERS_GET_ALL)]
        [Function("GetAllUser")]
        public async Task<HttpResponseData> GetAllUser([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RegisteredRoute.USERS_GET_ALL)] HttpRequestData req, string organizationId)
        {
            if (string.IsNullOrEmpty(organizationId))
                return await req.OkResult(data: null, status: "Bad Request", message: "Organization Id cannot be empty", statusCode: HttpStatusCode.BadRequest);

            _userService = _serviceFactory.GetUserService(organizationId);
            var users = await _userService.GetUsersAsync();

            var data = CustomMapper.Mapper.Map<List<GetUserResponseModel>>(users);

            return await req.OkResult(data);
        }

        [RouteMapper(RegisteredRoute.USERS_GET, RouteOperations.USERS_GET)]
        [Function("GetUser")]
        public async Task<HttpResponseData> GetUser([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RegisteredRoute.USERS_GET)] HttpRequestData req, string organizationId, string userId)
        {
            if (string.IsNullOrEmpty(organizationId) || string.IsNullOrEmpty(userId))
                return await req.OkResult(data: null, status: "Bad Request", message: "Organization Id/UserEntity Id cannot be empty", statusCode: HttpStatusCode.BadRequest);

            _userService = _serviceFactory.GetUserService(organizationId);
            var user = await _userService.GetUserAsync(userId);

            var data = CustomMapper.Mapper.Map<GetUserResponseModel>(user);

            return await req.OkResult(data);
        }

        [RouteMapper(RegisteredRoute.USERS_CREATE, RouteOperations.USERS_CREATE)]
        [Function("CreateUser")]
        public async Task<HttpResponseData> CreateUser([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RegisteredRoute.USERS_CREATE)] HttpRequestData req, string organizationId)
        {
            try
            {
                var organization = await _organizationService.GetOrganizationAsync(organizationId);

                if (organization is null)
                    return await req.BadRequestResult(message: "Organization does not exist");

                var request = await req.ValidateRequestAsync<CreateUserRequestModel>();

                if (!request.IsValid)
                    return await req.ValidationResult(request.ValidationErrors);

                CosmosUser user = CustomMapper.Mapper.Map<CosmosUser>(request.Value);

                /* Checking Exiting User In B2C And CosmosDb */
                var userService = _serviceFactory.GetUserService(organizationId);

                if (userService.UserExistsByEmail(user.Email) || (await _b2cUserService.UserExistsByEmail(user.Email)))
                    return await req.BadRequestResult(message: "User already exists");

                var userId = IdHelper.NewNanoId();

                var b2cUser = PrepareB2cUser(user, organizationId, userId);

                var createdB2cUser = await _b2cUserService.CreateAsync(b2cUser);

                if (createdB2cUser == null)
                {
                    _logger.LogCritical($"{user.Email} cannot be created in Azure B2C");
                    return await req.BadRequestResult("User cannot be added");
                }

                //Map B2c Object Id to IdB2c
                user.IdB2c = createdB2cUser.Id;
                user.Id = userId;

                await userService.CreateUserAsync(user);
                List<string> roles = _colloPermissionService.GetRolePermissionString(organization.Roles);

                //Creating assignment for the user
                var assignment = await _colloPermissionService.CreateAsync(organizationId, userId, roles);

                return await req.OkResult(message: "User successfully created");
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"User exception ex:{ex.Message}");
                throw;
            }
        }

        [RouteMapper(RegisteredRoute.USERS_DELETE, RouteOperations.USERS_DELETE)]
        [Function("DeleteUser")]
        public async Task<HttpResponseData> DeleteUser([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = RegisteredRoute.USERS_DELETE)] HttpRequestData req, string organizationId, string userId)
        {
            if (string.IsNullOrEmpty(organizationId) || string.IsNullOrEmpty(userId))
                return await req.BadRequestResult(data: null, status: "Bad Request", message: "Organization Id cannot be empty");

            _userService = _serviceFactory.GetUserService(organizationId);

            if (_userService.UserExists(userId))
            {
                var user = await _userService.GetUserAsync(userId);
                //Delete User from both B2C and CosmosDB
                await _b2cUserService.DeleteAsync(user.IdB2c);
                await _userService.DeleteUserAsync(user);
                return await req.OkResult();
            }

            return await req.OkResult(data: null, status: "NoContent", message: "No Content", statusCode: HttpStatusCode.NoContent);
        }

        #region Private Method(s)

        //private static B2CUserServiceConfiguration GetB2CUserServiceConfiguration()
        //{
        //    var config = new ConfigurationBuilder()
        //                   .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
        //                   .AddEnvironmentVariables()
        //                   .Build();

        //    return config.GetSection(B2CUserServiceConfiguration.B2CUserServiceSettings).Get<B2CUserServiceConfiguration>();
        //}

        //private static B2C.User GenerateB2cUser(CosmosUser user)
        //{
        //    var b2cConfig = GetB2CUserServiceConfiguration();

        //    var b2cUser = new B2C.User
        //    {
        //        UserPrincipalName = user.Email,
        //        DisplayName = user.DisplayName,
        //        AccountEnabled = true,
        //        MailNickname = user.Email.Split('@').FirstOrDefault(),
        //        PasswordProfile = new B2C.PasswordProfile
        //        {
        //            ForceChangePasswordNextSignIn = false,
        //            Password = B2CPasswordHelper.GenerateNewPassword(3, 3, 2)
        //        },
        //        PasswordPolicies = "DisablePasswordExpiration",
        //        Identities = new List<B2C.ObjectIdentity>
        //        {
        //            new B2C.ObjectIdentity()
        //            {
        //                SignInType = "emailAddress",
        //                Issuer = b2cConfig.Issuer,
        //                IssuerAssignedId = user.Email
        //            }
        //        }
        //    };

        //    return b2cUser;
        //}


        //private static Invitation PrepareInviteUserModel(CosmosUser user, string organizationId)
        //{
        //    var additionalData = new Dictionary<string, object>();
        //    additionalData.Add("Organization", organizationId);

        //    var invvitationUser = new Invitation
        //    {
        //        InviteRedirectUrl = "http://localhost:7071/",
        //        InvitedUserDisplayName = user.DisplayName,
        //        InvitedUserEmailAddress = user.Email,
        //        InvitedUserMessageInfo = new InvitedUserMessageInfo
        //        {
        //            CustomizedMessageBody = ""
        //        },
        //        SendInvitationMessage = true,
        //        AdditionalData = additionalData

        //    };

        //    return invvitationUser;

        //}

        public Microsoft.Graph.User PrepareB2cUser(CosmosUser user, string organizationId, string userId)
        {
            var additionalData = new Dictionary<string, object>();
            additionalData.Add($"extension_{_b2CUserServiceConfig.B2cExtensionAppClientId.Replace("-", "")}_OrgId", organizationId);
            additionalData.Add($"extension_{_b2CUserServiceConfig.B2cExtensionAppClientId.Replace("-", "")}_UserId", userId);

            /*
            * 3 = Uppercase, 3 = Lowercase, 2 = Neumeric
            * Can be changed accordingly.
            * To-Do: Send Temporary Password Via Email
            */
            var password = B2CPasswordHelper.GenerateNewPassword(3, 3, 2);

            var b2cuser = new Microsoft.Graph.User
            {
                AccountEnabled = true,
                DisplayName = user.DisplayName,
                OtherMails = new List<string> { user.Email },
                Mail = user.Email,
                UserPrincipalName = $"{Guid.NewGuid()}@{_b2CUserServiceConfig.Issuer}",
                //PasswordPolicies = "DisablePasswordExpiration",
                Identities = new List<ObjectIdentity>
                    {
                        new ObjectIdentity{ SignInType = "emailAddress", Issuer = _b2CUserServiceConfig.Issuer, IssuerAssignedId = user.Email },
                    },
                AdditionalData = additionalData,
                UserType = "Member",
                CreationType = "LocalAccount",
                GivenName = user.DisplayName,
                Surname = user.DisplayName,
                PasswordProfile = new PasswordProfile { ForceChangePasswordNextSignIn = false, ForceChangePasswordNextSignInWithMfa = false, Password = password },
            };

            return b2cuser;
        }

        #endregion
    }
}