using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Organizations;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Services.ServiceFactories.Organizations
{
    public class GatewayServiceFactory : SiteServiceFactory
    {
        private readonly IGatewayService _gatewayService;

        private readonly string _gatwayId;

        public Gateway Gateway { get; set; }

        public GatewayServiceFactory(IGatewayService gatewayService, ISiteService siteService, IOrganizationService organizationService, string siteId, string organizationId, string gatewayId) : base(siteService, organizationService, siteId, organizationId)
        {
            _gatewayService = gatewayService;
            _gatwayId = gatewayId;

            if (Gateway == null)
                Gateway = _gatewayService.GetGatewayAsync(_gatwayId).GetAwaiter().GetResult();
        }
    }
}
