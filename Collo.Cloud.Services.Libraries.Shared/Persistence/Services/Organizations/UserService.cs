using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Services.ServiceFactories;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Organizations
{
    public class UserService : OrganizationServiceFactory, IUserService
    {
        private readonly IOrganizationService _organizationService;

        public UserService(IOrganizationService organizationService, string organizationId) : base(organizationService, organizationId)
        {
            _organizationService = organizationService;
        }

        public async Task<User> GetUserAsync(string id)
        {
            return await Task.FromResult(result: Organization.Users.Where(x => x.Id == id).FirstOrDefault());
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await Task.FromResult(result: Organization.Users);
        }

        public async Task CreateUserAsync(User user)
        {
            Organization.Users.Add(user);
            await _organizationService.Update(Organization);
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = Organization.Users.Where(x => x.Id == id).FirstOrDefault();
            return await DeleteUserAsync(user);
        }

        public async Task<bool> DeleteUserAsync(User user)
        {
            if (user != null)
            {
                Organization.Users.Remove(user);
                await _organizationService.Update(Organization);
                return true;
            }

            return false;
        }

        public bool UserExists(string id)
        {
            if (Organization.Users.Where(x => x.Id == id).Any())
                return true;

            return false;
        }

        public bool UserExists(User user)
        {
            if (Organization.Users.Where(x => x.Id == user.Id).Any())
                return true;

            return false;
        }

        public bool UserExistsByEmail(string email)
        {
            if (Organization.Users.Where(x => x.Email == email).Any())
                return true;

            return false;
        }

        public async Task UpdateUser(User user)
        {
            if (user == null && !Organization.Users.Any(x => x.Id == user.Id))
                return;

            var currentUserIndex = Organization.Users.FindIndex(x => x.Id == user.Id);

            Organization.Users[currentUserIndex] = user;

            await _organizationService.Update(Organization);
        }

        public async Task<List<User>> GetUsersWithPermissionAsync(List<string> ids, bool isAny)
        {
            if (isAny == true)
                return await GetUsersAsync();

            List<User> users = new();

            foreach (var id in ids)
            {
                var user = Organization.Users.FirstOrDefault(x => x.Id == id);

                users.Add(user);
            }

            return users;
        }
    }
}
