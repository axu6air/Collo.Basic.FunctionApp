using Collo.Cloud.Services.Libraries.Shared.Infrastructures.Constants;
using Collo.Cloud.Services.Libraries.Shared.Permission.Enums;
using Collo.Cloud.Services.Libraries.Shared.Permission.Helpers;
using Collo.Cloud.Services.Libraries.Shared.Permission.Interfaces;
using Collo.Cloud.Services.Libraries.Shared.Permission.Mappers;
using Collo.Cloud.Services.Libraries.Shared.Permission.Models;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Assignment;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Repositories;
using Humanizer;
using Microsoft.Azure.Cosmos;
using PermissionResponse = Collo.Cloud.Services.Libraries.Shared.Permission.Models.PermissionResponse;

namespace Collo.Cloud.Services.Libraries.Shared.Permission
{
    public class ColloPermissionService : IColloPermissionService
    {
        private readonly IRepository<Assignment> _permissionRepo;
        private readonly PermissionAssignmentsService _assignmentTrie;

        public ColloPermissionService(
            IRepository<Assignment> permissionRepo
            , PermissionAssignmentsService assignmentTrie)
        {
            _permissionRepo = permissionRepo;
            _assignmentTrie = assignmentTrie;
            _assignmentTrie.Configure("organizations", 1, "any", '/');
        }

        public async Task<PermissionResponse> AddAsync(PermissionRequest request, string userId, string orgId)
        {
            var id = $"{orgId}::{userId}::1";
            var data = PermissionMapper.Mapper.Map<Assignment>(request);
            data.Id = id;
            data.Organization = orgId;
            var (result, _) = await _permissionRepo.CreateAsync(data);
            return PermissionMapper.Mapper.Map<PermissionResponse>(result);
        }

        public async Task<Assignment> CreateAsync(string organizationId, string userId)
            => await CreateAssignmentAsync(organizationId, userId);

        public async Task<Assignment> CreateAsync(string organizationId, string userId, List<string> Roles)
            => await CreateAssignmentAsync(organizationId, userId, Roles);

        public async Task<PermissionResponse> AddAssignmentsAsync(string assignment, string organizationId, string userId)
        {
            var assignments = await GetAsync(organizationId, userId);
            assignments.Assignments.Add(assignment); ;

            var result = PermissionMapper.Mapper.Map<Assignment>(assignments);
            await _permissionRepo.UpdateAsync(result);

            return PermissionMapper.Mapper.Map<PermissionResponse>(result);

        }

        public async Task<PermissionResponse> AddPermissionAsync(List<string> permissions, string organizationId, string userId)
        {
            var id = $"{organizationId}::{userId}::1";

            var request = new PermissionRequest
            {
                Roles = permissions
            };

            var data = PermissionMapper.Mapper.Map<Assignment>(request);
            data.Id = id;
            data.Organization = organizationId;
            var (result, _) = await _permissionRepo.CreateAsync(data);
            return PermissionMapper.Mapper.Map<PermissionResponse>(result);
        }

        public async Task<PermissionResponse> UpdateAsync(
            string userId,
            string orgId,
            IReadOnlyList<PatchOperation> patchOperations
        )
        {
            var id = $"{orgId}::{userId}::1";
            var (result, _) = await _permissionRepo.PatchAsync(id, patchOperations);
            return PermissionMapper.Mapper.Map<PermissionResponse>(result);
        }

        public async Task<PermissionResponse> UpdateRoles(string organizationId, string userId, List<string> permissions)
        {
            var id = $"{organizationId}::{userId}::1";
            var (result, _) = await _permissionRepo.PatchAsync(id, new[] {
                PatchOperation.Replace($"/{nameof(Assignment.Roles).Camelize()}", permissions)
            });

            if (result == null)
                return null;
            return PermissionMapper.Mapper.Map<PermissionResponse>(result);
        }

        public async Task<bool> DeleteAssignment(string assignment, string organizationId, string userId)
        {
            var assignments = await GetAsync(organizationId, userId);
            var hasAssignmemnt = assignments.Assignments.Any(x => x.Equals(assignment, StringComparison.OrdinalIgnoreCase));

            if (hasAssignmemnt)
                assignments.Assignments.Remove(assignment);
            else
                return false;

            var result = PermissionMapper.Mapper.Map<Assignment>(assignments);
            var response = await _permissionRepo.UpdateAsync(result);

            if (response.statusCode == System.Net.HttpStatusCode.OK)
                return true;

            return false;
        }

        public async Task<PermissionResponse> AddRoles(string organizationId, string userId, List<string> permission)
        {

            var assignment = await GetAsync(organizationId, userId);
            assignment.Roles.AddRange(permission);

            var result = PermissionMapper.Mapper.Map<Assignment>(assignment);
            await _permissionRepo.UpdateAsync(result);

            return PermissionMapper.Mapper.Map<PermissionResponse>(result);
        }

        public async Task<PermissionResponse> UpdateAssignments(string userId, string organizationId, List<string> Assignments)
        {
            var id = $"{organizationId}::{userId}::1";

            var (result, _) = await _permissionRepo.PatchAsync(id, new[] {
                PatchOperation.Replace($"/{nameof(Assignment.Assignments).Camelize()}", Assignments)
            });
            return PermissionMapper.Mapper.Map<PermissionResponse>(result);
        }

        /// <summary>
        /// Get Permission Assignment of a specific user.
        /// </summary>
        /// <param name="userId">The user to get Permission Assignment for</param>
        /// <returns></returns>
        public async Task<PermissionResponse> GetAsync(string organizationId, string userId)
        {
            var id = $"{organizationId}::{userId}::1";
            var (result, _) = await _permissionRepo.GetAsync(id);
            return PermissionMapper.Mapper.Map<PermissionResponse>(result); ;
        }

        /// <summary>
        /// Get the Permission Assignment of currently logged in user.
        /// </summary>
        /// <returns></returns>
        public async Task<PermissionResponse> GetAsync()
        {
            var id = PartitionKeyHelper.GenerateId(formator: "::", new string[] { "extension_OrgId", "extension_UserId" });
            var (result, _) = await _permissionRepo.GetAsync(id);
            return PermissionMapper.Mapper.Map<PermissionResponse>(result); ;
        }

        public async Task DeleteAsync(string organizationId, string userId)
        {
            var id = $"{organizationId}::{userId}::1";
            await _permissionRepo.DeleteAsync(id);
        }

        public async Task<bool> HasPermissionAsync(
            List<string> roles,
            List<string> scopes,
            List<PermissionEnum> permissions)
        {
            var result = await GetAsync();
            if (result == null)
                return false;

            _assignmentTrie.Roles.Clear();
            _assignmentTrie.Roles.AddRange(result.Roles);
            AssignmentNode node = _assignmentTrie.Search(scopes.FirstOrDefault());

            //To-Do: Compare PermissionEnum(s), return true if match.
            return node.Permissions.Any(x => x.Equals(permissions.FirstOrDefault()));
        }

        public async Task<List<PermissionEnum>> HasPermissionAsync(string service, string resource)
        {
            var result = await GetAsync();
            if (result == null)
                return null;
            _assignmentTrie.Roles.AddRange(result.Roles);

            AssignmentNode node = _assignmentTrie.Search(resource);
            return node?.Permissions;
        }

        public async Task LoadAssignmentsAsync(string query)
        {
            _assignmentTrie.RoleResourcesList.Clear();
            var colloPermissionData = await _permissionRepo.GetByQueryAsync(query);
            foreach (var colloPermission in colloPermissionData.results)
            {
                foreach (var assignment in colloPermission.Assignments)
                {
                    //assignment= "reporting/user::/orgzanizations/299392388/sites/92939304/devices/any/features/any",
                    var splitData = assignment.Split("::");
                    var role = splitData[0];//-> reporting/user
                    var resource = splitData[1].Remove(0, 1);//->organizations/299392388/sites/92939304/devices/any/features/any
                    _assignmentTrie.RoleResourcesList.Add((role, resource));
                }
                foreach (var roleResource in _assignmentTrie.RoleResourcesList)
                    _assignmentTrie.Insert(roleResource.Resource);
            }
        }

        public async Task<List<(string role, string services)>> GetRolesAndPermissions(PermissionResponse response)
        {

            List<(string role, string services)> rolesAndAssignments = new();

            foreach (var item in response.Assignments)
            {
                var split = item.Split(response.Roleassignmentseparator);
                (string role, string service) data = (split[0], split[1]);
                rolesAndAssignments.Add(data);
            }

            return await Task.FromResult(rolesAndAssignments);
        }

        public async Task<List<(string role, string permission)>> GetRoles(PermissionResponse response)
        {
            if (response == null)
                return new List<(string role, string permission)>();

            List<(string role, string permission)> rolesAndPermissions = new();

            foreach (var item in response.Roles)
            {
                var splitData = item.Split(response.Rolepermissionseparator);
                rolesAndPermissions.Add((splitData[0], splitData[1]));
            }

            return await Task.FromResult(rolesAndPermissions);

        }

        public async Task<string[]> GetAssignments(PermissionResponse response)
        {
            List<string> data = new();

            foreach (var item in response.Assignments)
            {
                var split = item.Split(response.Roleassignmentseparator);
                data.Add(split[1]);
            }

            return await Task.FromResult(data.ToArray());
        }

        public async Task<List<(string assignment, List<char> permissions)>> GetAssigmnmentsAndPermissions(PermissionResponse response)
        {

            Dictionary<string, string> data = new();

            foreach (var item in response.Roles)
            {
                var splitData = item.Split(response.Rolepermissionseparator);
                data.Add(splitData[0], splitData[1]);
            }

            List<(string assignment, List<char> permissions)> assigmnmentsAndPermissions = new();

            foreach (var item in response.Assignments)
            {
                var splittedAssignments = item.Split(response.Roleassignmentseparator);

                if (data.TryGetValue(splittedAssignments[0], out string permissionString))
                {
                    var permissions = permissionString.ToCharArray().ToList();
                    var assignmentAndPermission = (splittedAssignments[1], permissions);
                    assigmnmentsAndPermissions.Add(assignmentAndPermission);
                }
            }

            return await Task.FromResult(assigmnmentsAndPermissions);
        }

        public List<string> GetRolePermissionString(List<Role> roles)
        {
            if (roles == null)
                return null;

            List<string> rolePermissions = new();

            foreach (var role in roles)
            {
                var permissions = new string(role.Permissions.ToArray());
                string rolePermission = role.Name + AssignmentConstants.ROLE_PERMISSION_SEPARATOR + permissions;
                rolePermissions.Add(rolePermission);
            }

            return rolePermissions.Count > 0 ? rolePermissions : null;
        }

        private async Task<Assignment> CreateAssignmentAsync(string organizationId, string userId, List<string> Roles = null)
        {
            var id = $"{organizationId}::{userId}::1";
            Assignment assignment = new();
            assignment.Id = id;

            if (Roles != null)
                assignment.Roles = Roles;

            var (result, statusCode) = await _permissionRepo.CreateAsync(assignment);

            if (statusCode == System.Net.HttpStatusCode.OK)
                return result;

            return null;
        }

    }
}
