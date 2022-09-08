using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Organizations;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Services.ServiceFactories
{
    public class SiteServiceFactory : OrganizationServiceFactory
    {
        private readonly ISiteService _siteService;
        private readonly IOrganizationService _organizationService;

        private readonly string _siteId;
        private readonly string _organizationId;

        public Site Site { get; set; }

        public SiteServiceFactory(ISiteService siteService, IOrganizationService organizationService, string siteId, string organizationId) : base(organizationService, organizationId)
        {
            _siteService = siteService;
            _organizationId = organizationId;
            _organizationService = organizationService;
            _siteId = siteId;

            if (Site == null)
                Site = _siteService.GetSiteAsync(_siteId).GetAwaiter().GetResult();
        }

        public IGatewayService GetGatewayService()
        {
            if (string.IsNullOrEmpty(_organizationId) || string.IsNullOrEmpty(_siteId))
                return null;

            return new GatewayService(_siteService, _organizationService, _siteId, _organizationId);
        }
    }
}
