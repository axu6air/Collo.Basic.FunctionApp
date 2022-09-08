using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Organizations
{
    public interface IGatewayService
    {
        Task CreateGatewayAsync(Gateway site);
        Task<Gateway> GetGatewayAsync(string id);
        Task<List<Gateway>> GetAllGatewaysAsync();
        Task<bool> DeleteGatewayAsync(Gateway gateway);
        Task<bool> DeleteGatewayAsync(string id);
        bool ExistsGateway(Gateway gateway);
        bool ExistsGateway(string id);
        bool ExistsByName(string name);
        Task UpdateGatewayAsync(Gateway gateway);
        Task<List<Gateway>> GetGatewaysWithPermissionAsync(List<string> ids, bool isAny);
    }
}
