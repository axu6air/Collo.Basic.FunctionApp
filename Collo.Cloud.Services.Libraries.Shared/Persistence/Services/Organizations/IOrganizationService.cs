using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization;
using Microsoft.Azure.Cosmos;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Organizations
{
    public interface IOrganizationService
    {
        ValueTask<Organization> GetOrganizationAsync(string id);
        ValueTask<List<Organization>> GetOrganizationAsync();
        ValueTask Update(Organization organization);
        ValueTask<Organization> CreateAsync(Organization organization);
        ValueTask<Organization> PatchAsync(string id, IReadOnlyList<PatchOperation> patchOperations);
        ValueTask<bool> ExistsByName(string name);
        ValueTask<bool> DeleteOrganization(string organizationId);
        ValueTask<List<Organization>> GetOrganizationsWithPermissionAsync(List<string> ids, bool isAny);
    }
}
