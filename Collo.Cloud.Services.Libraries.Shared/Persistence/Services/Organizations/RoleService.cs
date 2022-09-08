using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Services.ServiceFactories;
using Humanizer;
using Microsoft.Azure.Cosmos;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Organizations
{
    public class RoleService : OrganizationServiceFactory, IRoleService
    {
        private readonly IOrganizationService _organizationService;

        public RoleService(IOrganizationService organizationService, string organizationId) : base(organizationService, organizationId)
        {
            _organizationService = organizationService;
        }

        public async Task<List<Role>> CreateRole(string organizationId, Role role)
        {
            var response = await _organizationService.PatchAsync(organizationId,
               new[] { PatchOperation.Add($"/{nameof(Organization.Roles).Camelize()}/-", role) }
             );

            if (response == null)
                return null;

            return response.Roles;
        }

        public async Task<bool> DeleteRole(string organizationId, string roleId)
        {

            var org = await _organizationService.GetOrganizationAsync(organizationId);
            if (org == null)
                return false;

            var roleIndex = org.Roles.FindIndex(r => r.Id == roleId);
            if (roleIndex == -1)
                return false;

            var response = await _organizationService.PatchAsync(organizationId,
                new[] { PatchOperation.Replace($"/{nameof(Organization.Roles).Camelize()}/{roleIndex}/isDeleted", true) }
            );

            if (response != null)
                return true;

            return false;

        }

        public async Task<Role> GetRole(string organizationId, string roleId)
        {
            var org = await _organizationService.GetOrganizationAsync(organizationId);

            if (org == null)
                return null;

            var role = org.Roles.FirstOrDefault(x => x.Id == roleId);

            if (role == null)
                return null;

            return role;
        }

        public async Task<List<Role>> GetRoles(string organizationId)
        {
            var org = await _organizationService.GetOrganizationAsync(organizationId);
            if (org == null)
                return null;

            var roles = org.Roles;
            if (roles == null)
                return null;

            return roles;

        }

        public async Task<List<Role>> UpdateRole(string organizationId, string roleId, Role role)
        {
            var organization = await _organizationService.GetOrganizationAsync(organizationId);

            if (organization == null)
                return null;

            var roleIndex = organization.Roles.FindIndex(x => x.Id == roleId);

            if (roleIndex == -1)
                return null;

            var response = await _organizationService.PatchAsync(organizationId,
                new[] { PatchOperation.Replace($"/{nameof(Organization.Roles).Camelize()}/{roleIndex}/{nameof(Role.Name).Camelize()}", role.Name) });

            if (response == null)
                return null;

            return response.Roles;
        }

        public async Task<List<Role>> GetStaticRoles()
        {
            Role adminRole = new()
            {
                Id = await Nanoid.Nanoid.GenerateAsync(),
                IsDeleted = false,
                Name = "admin",
                DisplayName = "Admin",
                Permissions = new List<char> { 'c', 'r', 'u', 'd', 'l' }
            };

            Role userRole = new()
            {
                Id = await Nanoid.Nanoid.GenerateAsync(),
                IsDeleted = false,
                Name = "user",
                DisplayName = "User",
                Permissions = new List<char> { 'r', 'l' }
            };

            List<Role> roles = new() { adminRole, userRole };

            return roles;
        }
    }
}
