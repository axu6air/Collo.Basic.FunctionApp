using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Organizations
{
    public interface ISiteService
    {
        Task<Site> GetSiteAsync(string id);
        Task<List<Site>> GetAllSitesAsync();
        Task CreateSiteAsync(Site site);
        Task<bool> DeleteSiteAsync(string id);
        Task<bool> DeleteSiteAsync(Site site);
        bool SiteExists(Site site);
        bool SiteExists(string id);
        bool ExistsByName(string name);
        Task UpdateSiteAsync(Site site);
        Task<List<Site>> GetSitesWithPermissionsAsync(List<string> ids, bool isAny);
    }
}
