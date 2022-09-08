namespace Collo.Cloud.IAM.Api.Routes
{
    public class RegisteredRoute
    {
        #region organization

        public const string ORGANIATION_CREATE = "iam/organizations";
        public const string ORGANIZATION_UPDATE = "iam/organizations/{organizationId}";
        public const string ORGANIZATION_DELETE = "iam/organizations/{organizationId}";
        public const string ORGANIZATION_GET_ALL = "iam/organizations";
        public const string ORGANIZATION_GET = "iam/organizations/{organizationId}";

        #endregion

        #region permission

        public const string PERMISSION_CREATE_ASSIGNMENT = "iam/organizations/users/assignments";
        public const string PERMISSION_ADD_ASSIGNMENT = "iam/organizations/{organizationId}/users/{userId}/assignments";
        public const string PERMISSION_GET_ASSIGNMENT = "iam/organizations/{organizationId}/users/{userId}/assignments";
        public const string PERMISSION_ADD_ROLES = "iam/organizations/{organizationId}/users/{userId}/roles";
        public const string PERMISSION_UPDATE_ASSIGNMENT = "iam/organizations/{organizationId}/users/{userId}/assignments";
        public const string PERMISSION_GET_ALL = "iam/organizations/users/permissions";
        public const string PERMISSION_CREATE_ROLE_PERMISSION = "iam/organizations/{organizationId}/users/{userId}/roles";
        public const string PERMISSION_DELETE_ASSIGNMENT = "iam/organizations/{organizationId}/users/{userId}/assignments";
        #endregion

        #region user

        public const string USERS_GET_ALL = "iam/organizations/{organizationId}/users";
        public const string USERS_GET = "iam/organizations/{organizationId}/users/{userId}";
        public const string USERS_CREATE = "iam/organizations/{organizationId}/users";
        public const string USERS_DELETE = "iam/organizations/{organizationId}/users/{userId}";

        #endregion

        #region role

        public const string ROLE_GET_ALL = "iam/organizations/{organizationId}/roles";
        public const string ROLE_CREATE = "iam/organizations/{organizationId}/roles";
        public const string ROLE_DELETE = "iam/organizations/{organizationId}/roles/{roleId}";
        public const string ROLE_GET = "iam/organizations/{organizationId}/roles/{roleId}";
        public const string ROLE_UPDATE = "iam/organizations/{organizationId}/roles/{roleId}";

        #endregion

        #region site

        public const string SITE_GET_ALL = "dm/organizations/{organizationId}/sites";
        public const string SITE_GET = "dm/organizations/{organizationId}/sites/{siteId}";
        public const string SITE_CREATE = "dm/organizations/{organizationId}/sites";
        public const string SITE_DELETE = "dm/organizations/{organizationId}/sites/{siteId}";
        public const string SITE_UPDATE = "dm/organizations/{organizationId}/sites/{siteId}";

        #endregion

        #region gateway

        public const string GATEWAY_CREATE = "dm/organizations/{organizationId}/sites/{siteId}/gateways";
        public const string GATEWAY_GET_ALL = "dm/organizations/{organizationId}/sites/{siteId}/gateways";
        public const string GATEWAY_GET = "dm/organizations/{organizationId}/sites/{siteId}/gateways/{gatewayId}";
        public const string GATEWAY_DELETE = "dm/organizations/{organizationId}/sites/{siteId}/gateways/{gatewayId}";
        public const string GATEWAY_UPDATE = "dm/organizations/{organizationId}/sites/{siteId}/gateways/{gatewayId}";

        #endregion

        #region allocated instrument

        public const string ALLOCATE_INSTRUMENT_ADD = "dm/organizations/{organizationId}/sites/{siteId}/gateways/{gatewayId}/instruments";
        public const string ALLOCATE_INSTRUMENTS_GET = "dm/organizations/{organizationId}/sites/{siteId}/gateways/{gatewayId}/instruments";
        public const string ALLOCATE_INSTRUMENTS_DELETE = "dm/organizations/{organizationId}/sites/{siteId}/gateways/{gatewayId}/instruments/{instrumentId}";

        #endregion

        #region instrument

        public const string INSTRUMENT_ADD = "dm/instruments";
        public const string INSTRUMENT_UPDATE = "dm/instruments/{instrumentId}";
        public const string INSTRUMENT_GET_ALL = "dm/instruments";
        public const string INSTRUMENT_DELETE = "dm/instruments/{instrumentId}";
        public const string INSTRUMENT_GET = "dm/instruments/{instrumentId}";
        public const string INSTRUMENT_GET_UNALLOCATEDINSTRUMENT = "dm/unallocatedinstruments";

        #endregion
    }

    public class RouteOperations
    {
        #region organization

        public const string ORGANIATION_CREATE = "create";
        public const string ORGANIZATION_UPDATE = "update";
        public const string ORGANIZATION_DELETE = "delete";
        public const string ORGANIZATION_GET_ALL = "list";
        public const string ORGANIZATION_GET = "read";

        #endregion

        #region permission

        public const string PERMISSION_CREATE_ASSIGNMENT = "create";
        public const string PERMISSION_GET_ASSIGNMENT = "read";
        public const string PERMISSION_ADD_ROLES = "update";
        public const string PERMISSION_UPDATE_ASSIGNMENT = "update";
        public const string PERMISSION_GET_PERMISSION_LIST = "list";
        public const string PERMISSION_CREATE_ROLE_PERMISSION = "create";
        public const string PERMISSION_DELETE_ASSIGNMENT = "delete";
        public const string PERMISSION_ADD_ASSIGNMENT = "update";

        #endregion

        #region user

        public const string USERS_GET_ALL = "list";
        public const string USERS_GET = "read";
        public const string USERS_CREATE = "create";
        public const string USERS_DELETE = "delete";

        #endregion

        #region role

        public const string ROLE_GET_ALL = "list";
        public const string ROLE_CREATE = "create";
        public const string ROLE_DELETE = "delete";
        public const string ROLE_GET = "read";
        public const string ROLE_UPDATE = "update";

        #endregion

        #region site

        public const string SITE_GET_ALL = "list";
        public const string SITE_GET = "read";
        public const string SITE_CREATE = "create";
        public const string SITE_DELETE = "delete";
        public const string SITE_UPDATE = "update";

        #endregion

        #region gateway

        public const string GATEWAY_CREATE = "create";
        public const string GATEWAY_GET_ALL = "list";
        public const string GATEWAY_GET = "read";
        public const string GATEWAY_DELETE = "delete";
        public const string GATEWAY_UPDATE = "update";

        #endregion

        #region allocated instrument

        public const string ALLOCATE_INSTRUMENTS_ADD = "create";
        public const string ALLOCATE_INSTRUMENTS_GET = "list";
        public const string ALLOCATE_INSTRUMENTS_DELETE = "delete";

        #endregion

        #region instrument

        public const string INSTRUMENT_ADD = "create";
        public const string INSTRUMENT_UPDATE = "update";
        public const string INSTRUMENT_GET_ALL = "list";
        public const string INSTRUMENT_DELETE = "delete";
        public const string INSTRUMENT_GETBYID = "read";
        public const string INSTRUMENT_GET_UNALLOCATEDINSTRUMENT = "list";

        #endregion

    }
}

