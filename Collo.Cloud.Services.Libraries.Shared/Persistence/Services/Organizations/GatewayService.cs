using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Services.ServiceFactories;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Organizations
{
    public class GatewayService : SiteServiceFactory, IGatewayService
    {
        private readonly ISiteService _siteService;

        public GatewayService(ISiteService siteService, IOrganizationService organizationService, string siteId, string organizationId) : base(siteService, organizationService, siteId, organizationId)
        {
            _siteService = siteService;
        }

        public async Task CreateGatewayAsync(Gateway gateway)
        {
            if (Site == null)
                return;

            Site.Gateways.Add(gateway);
            await _siteService.UpdateSiteAsync(Site);
        }

        public async Task<bool> DeleteGatewayAsync(Gateway gateway)
        {
            if (Site == null)
                return false;

            if (gateway != null)
            {
                Site.Gateways.Remove(gateway);
                await _siteService.UpdateSiteAsync(Site);
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteGatewayAsync(string id)
        {
            var gatewayEntity = Site.Gateways.FirstOrDefault(x => x.Id == id);
            return await DeleteGatewayAsync(gatewayEntity);
        }

        public bool ExistsGateway(Gateway gateway)
        {
            if (Site == null)
                return false;

            if (Site.Gateways.Where(x => x.Id == gateway.Id).Any())
                return true;

            return false;
        }

        public bool ExistsGateway(string id)
        {
            if (Site == null)
                return false;

            if (Site.Gateways.Where(x => x.Id == id).Any())
                return true;

            return false;
        }

        public async Task<List<Gateway>> GetAllGatewaysAsync()
        {
            if (Site == null)
                return null;

            return await Task.FromResult(result: Site.Gateways);
        }

        public async Task<List<Gateway>> GetGatewaysWithPermissionAsync(List<string> ids, bool isAny)
        {
            if (isAny == true)
                return await GetAllGatewaysAsync();

            List<Gateway> gatewayList = new();

            foreach (var id in ids)
            {
                var gateway = Site.Gateways.FirstOrDefault(x => x.Id == id);
                gatewayList.Add(gateway);
            }

            return gatewayList;
        }

        public async Task<Gateway> GetGatewayAsync(string id)
        {
            if (Site == null)
                return null;

            return await Task.FromResult(result: Site.Gateways.FirstOrDefault(x => x.Id == id));
        }

        public async Task UpdateGatewayAsync(Gateway gateway)
        {
            if (Site == null)
                return;

            if (gateway == null)
                return;

            var currentGateway = Site.Gateways.Where(x => x.Id == gateway.Id).FirstOrDefault();

            if (currentGateway == null)
                return;

            currentGateway = gateway;

            await _siteService.UpdateSiteAsync(Site);
        }

        public bool ExistsByName(string name)
        {
            if (Site == null)
                return false;

            if (Site.Gateways.Where(x => x.Name == name).Any())
                return true;

            return false;
        }
    }
}
