using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Organizations
{
    public interface IRoleService
    {
        Task<List<Role>> CreateRole(string organizationId, Role role);
        Task<bool> DeleteRole(string organizationId, string roleId);
        Task<List<Role>> UpdateRole(string organizationId, string roleId, Role role);
        Task<Role> GetRole(string organizationId, string roleId);
        Task<List<Role>> GetRoles(string organizationId);
        Task<List<Role>> GetStaticRoles();
    }
}
