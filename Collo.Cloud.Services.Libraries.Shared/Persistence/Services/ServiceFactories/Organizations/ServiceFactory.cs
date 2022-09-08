using Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Organizations;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Services
{
    public class ServiceFactory
    {
        private readonly IOrganizationService _organizationService;

        public ServiceFactory(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        public IUserService GetUserService(string organizationId)
        {
            if (string.IsNullOrEmpty(organizationId))
                return null;

            return new UserService(_organizationService, organizationId);
        }

        public IRoleService GetRoleService(string organizationId)
        {
            if (string.IsNullOrEmpty(organizationId))
                return null;

            return new RoleService(_organizationService, organizationId);
        }

        public ISiteService GetSiteService(string organizationId)
        {
            if (string.IsNullOrEmpty(organizationId))
                return null;

            return new SiteService(_organizationService, organizationId);
        }

        public IGatewayService GetGatewayService(string organizationId, string siteId)
        {
            if (string.IsNullOrEmpty(organizationId) || string.IsNullOrEmpty(siteId))
                return null;

            var siteService = GetSiteService(organizationId);

            return new GatewayService(siteService, _organizationService, siteId, organizationId);
        }

        public IAllocatedInstrumentService GetAllocatedInstrumentService(string organizationId, string siteId, string gatewayId)
        {
            if (string.IsNullOrEmpty(organizationId) || string.IsNullOrEmpty(siteId) || string.IsNullOrEmpty(gatewayId))
                return null;

            var siteService = GetSiteService(organizationId);

            var gatewayService = GetGatewayService(organizationId, siteId);

            return new AllocatedInstrumentService(gatewayService, siteService, _organizationService, siteId, organizationId, gatewayId);
        }


    }
}
