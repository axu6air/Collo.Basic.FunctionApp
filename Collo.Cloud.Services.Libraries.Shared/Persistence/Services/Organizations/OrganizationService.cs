using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Repositories;
using Humanizer;
using Microsoft.Azure.Cosmos;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Organizations
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IRepository<Organization> _organizationRepository;

        public OrganizationService(IRepository<Organization> organizationRepository)
        {
            _organizationRepository = organizationRepository;
        }

        public async ValueTask<Organization> GetOrganizationAsync(string id)
        {
            var (organization, _) = await _organizationRepository.GetAsync(id);

            return organization;
        }

        public async ValueTask<List<Organization>> GetOrganizationAsync()
        {
            //ToDo: Remove Raw Query
            //return await _organizationRepository.GetByQueryAsync($"SELECT x.{nameof(Organization.Id).ToLower()}, x.{nameof(Organization.Name).ToLower()} FROM {nameof(Organization)} x");
            var (organizations, _) = await _organizationRepository.GetByQueryAsync($"SELECT * FROM {nameof(Organization)} x");
            return organizations;
        }

        public async ValueTask Update(Organization organization)
        {
            await _organizationRepository.UpdateAsync(organization);
        }

        public async ValueTask<Organization> CreateAsync(Organization organization)
        {
            var (createOrganzation, _) = await _organizationRepository.CreateAsync(organization);

            return createOrganzation;
        }

        public async ValueTask<Organization> PatchAsync(string id, IReadOnlyList<PatchOperation> patchOperations)
        {
            var (organization, _) = await _organizationRepository.PatchAsync(id, patchOperations);

            return organization;
        }

        public async ValueTask<bool> ExistsByName(string name)
        {
            var (data, _) = await _organizationRepository.GetAsync(x => x.Name == name);
            return data.Any();
        }

        public async ValueTask<bool> DeleteOrganization(string organizationId)
        {
            var (response, _) = await _organizationRepository.PatchAsync(organizationId, new[] {
                PatchOperation.Replace($"/{nameof(Organization.IsDeleted).Camelize()}", true)
            });

            if (response == null)
                return false;

            return true;
        }

        public async ValueTask<List<Organization>> GetOrganizationsWithPermissionAsync(List<string> ids, bool isAny)
        {
            if (isAny == true)
                return await GetOrganizationAsync();

            List<Organization> organizations = new();

            foreach (var id in ids)
            {
                var organization = await GetOrganizationAsync(id);

                organizations.Add(organization);
            }

            return organizations;
        }
    }
}
