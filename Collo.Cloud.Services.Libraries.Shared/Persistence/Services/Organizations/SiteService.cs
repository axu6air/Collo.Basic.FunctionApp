using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Services.ServiceFactories;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Organizations
{
    public class SiteService : OrganizationServiceFactory, ISiteService
    {
        private readonly IOrganizationService _organizationService;

        public SiteService(IOrganizationService organizationService, string organizationId) : base(organizationService, organizationId)
        {
            _organizationService = organizationService;
        }

        public async Task CreateSiteAsync(Site site)
        {
            if (site == null)
                return;

            Organization.Sites.Add(site);
            await _organizationService.Update(Organization);
        }

        public async Task<bool> DeleteSiteAsync(string id)
        {
            var siteEntity = Organization.Sites.Where(x => x.Id == id).FirstOrDefault();

            return await DeleteSiteAsync(siteEntity);
        }

        public async Task<bool> DeleteSiteAsync(Site site)
        {
            if (site != null)
            {
                Organization.Sites.Remove(site);
                await _organizationService.Update(Organization);
                return true;
            }

            return false;
        }

        public async Task<List<Site>> GetAllSitesAsync()
        {
            var sites = Organization.Sites;

            if (sites == null)
                sites = new List<Site>();

            return await Task.FromResult(sites);
        }

        public async Task<List<Site>> GetAllSitesAsync(List<string> ids)
        {
            var sites = Organization.Sites;

            if (sites == null)
                sites = new List<Site>();

            return await Task.FromResult(sites);
        }

        public async Task<Site> GetSiteAsync(string id)
        {
            var site = Organization.Sites.FirstOrDefault(x => x.Id == id);

            if (site != null)
                return await Task.FromResult(site);

            return null;
        }

        public bool SiteExists(Site site)
        {
            if (Organization.Sites.Where(x => x.Id == site.Id).Any())
                return true;

            return false;
        }

        public bool SiteExists(string id)
        {
            if (Organization.Sites.Where(x => x.Id == id).Any())
                return true;

            return false;
        }

        public bool ExistsByName(string name)
        {
            if (Organization.Sites.Where(x => x.Name == name).Any())
                return true;

            return false;
        }

        public async Task UpdateSiteAsync(Site site)
        {
            if (site == null)
                return;

            var currentSite = Organization.Sites.Where(x => x.Id == site.Id).FirstOrDefault();

            if (currentSite == null)
                return;

            currentSite = site;

            await _organizationService.Update(Organization);
        }

        public async Task<List<Site>> GetSitesWithPermissionsAsync(List<string> ids, bool isAny)
        {
            if (isAny == true)
                return await GetAllSitesAsync();

            List<Site> sitesWithoutAny = new();

            foreach (var id in ids)
            {
                var site = Organization.Sites.Where(x => x.Id == id).FirstOrDefault();
                sitesWithoutAny.Add(site);
            }

            return sitesWithoutAny;
        }
    }
}
