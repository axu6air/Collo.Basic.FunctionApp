using Collo.Cloud.Services.Libraries.Shared.Permission.Enums;
using Collo.Cloud.Services.Libraries.Shared.Permission.Models;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Assignment;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization;

namespace Collo.Cloud.Services.Libraries.Shared.Permission.Interfaces
{
    public interface IColloPermissionService
    {
        Task<PermissionResponse> GetAsync(string organizationId, string userId);
        Task<PermissionResponse> GetAsync();
        Task<PermissionResponse> AddAsync(PermissionRequest request, string userId, string orgId);
        Task<Assignment> CreateAsync(string organizationId, string userId);
        Task<Assignment> CreateAsync(string organizationId, string userId, List<string> Roles);
        Task<PermissionResponse> AddRoles(string organizationId, string userId, List<string> permission);
        Task<PermissionResponse> UpdateRoles(string organizationId, string userId, List<string> permission);
        Task<PermissionResponse> UpdateAsync(string userId, string orgId, IReadOnlyList<Microsoft.Azure.Cosmos.PatchOperation> patchOperations);
        Task DeleteAsync(string userId, string orgId);
        Task<List<PermissionEnum>> HasPermissionAsync(string service, string resource);
        Task<bool> HasPermissionAsync(
            List<string> roles,
            List<string> scopes,
            List<PermissionEnum> permissions);
        Task<PermissionResponse> AddPermissionAsync(List<string> permissions, string organizationId, string userId);
        Task<PermissionResponse> UpdateAssignments(string userId, string organizationId, List<string> Assignments);
        Task<List<(string role, string services)>> GetRolesAndPermissions(PermissionResponse response);
        Task<List<(string role, string permission)>> GetRoles(PermissionResponse response);
        Task<string[]> GetAssignments(PermissionResponse response);

        Task<List<(string assignment, List<char> permissions)>> GetAssigmnmentsAndPermissions(PermissionResponse response);
        Task<PermissionResponse> AddAssignmentsAsync(string assignment, string organizationId, string userId);
        Task<bool> DeleteAssignment(string assignment, string organizationId, string userId);
        List<string> GetRolePermissionString(List<Role> roles);
    }
}
