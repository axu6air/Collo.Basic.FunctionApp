using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization;

namespace Collo.Cloud.Services.Libraries.Shared.Helpers
{
    public static class InstrumentAllocationHelper
    {
        public static string GetAllocationString(string siteId, string gatewayId, string instrumentId)
        {
            return $"site:{siteId}:gateway:{gatewayId}:instrument:{instrumentId}";
        }

        public static List<string> GetAllocationString(string siteId, string gatewayId, List<AllocatedInstrument> instruments)
        {
            var allocations = new List<string>();

            foreach (var instrument in instruments)
            {
                allocations.Add($"site:{siteId}:gateway:{gatewayId}:instrument:{instrument.Id}");
            }
            return allocations;
        }
    }
}
