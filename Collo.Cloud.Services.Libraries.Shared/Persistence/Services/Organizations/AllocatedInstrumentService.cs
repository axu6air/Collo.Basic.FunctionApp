using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Services.ServiceFactories.Organizations;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Organizations
{
    public class AllocatedInstrumentService : GatewayServiceFactory, IAllocatedInstrumentService
    {
        private readonly IGatewayService _gatewayService;

        public AllocatedInstrumentService(IGatewayService gatewayService, ISiteService siteService, IOrganizationService organizationService,
            string siteId, string organizationId, string gatewayId) : base(gatewayService, siteService, organizationService, siteId, organizationId, gatewayId)
        {
            _gatewayService = gatewayService;
        }

        public async Task AllocateInstrument(AllocatedInstrument allocatedInstrument)
        {

            if (Gateway == null)
                return;

            Gateway.AllocatedInstruments.Add(allocatedInstrument);
            await _gatewayService.UpdateGatewayAsync(Gateway);
        }

        public async Task AllocateInstrument(List<AllocatedInstrument> allocatedInstruments)
        {
            if (Gateway == null)
                return;

            Gateway.AllocatedInstruments.AddRange(allocatedInstruments);
            await _gatewayService.UpdateGatewayAsync(Gateway);
        }

        public async Task DeleteInstrument(string id)
        {
            if (Gateway == null)
                return;

            var allocatedInstrumentIndex = Gateway.AllocatedInstruments.FindIndex(x => x.Id == id);

            if (allocatedInstrumentIndex == -1)
                return;

            Gateway.AllocatedInstruments[allocatedInstrumentIndex].IsDeleted = true;

            await _gatewayService.UpdateGatewayAsync(Gateway);

        }

        public async Task<List<AllocatedInstrument>> GetAllocatedInstruments()
        {
            if (Gateway == null)
                return null;

            return await Task.FromResult(Gateway.AllocatedInstruments);
        }

        public async Task<List<AllocatedInstrument>> GetAllocatedInstrumentWithPermissionAsync(List<string> ids, bool isAny)
        {
            if (isAny == true)
                return await GetAllocatedInstruments();

            List<AllocatedInstrument> allocatedInstruments = new();

            foreach (var id in ids)
            {
                var allocatedInstrument = Gateway.AllocatedInstruments.FirstOrDefault(x => x.Id == id);
                allocatedInstruments.Add(allocatedInstrument);
            }

            return allocatedInstruments;
        }
    }
}
