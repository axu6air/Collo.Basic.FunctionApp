using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Organizations
{
    public interface IUserService
    {
        Task<User> GetUserAsync(string id);
        Task<List<User>> GetUsersAsync();
        Task CreateUserAsync(User user);
        Task<bool> DeleteUserAsync(string id);
        Task<bool> DeleteUserAsync(User user);
        bool UserExists(User user);
        bool UserExists(string id);
        bool UserExistsByEmail(string email);
        Task UpdateUser(User updateUser);
        Task<List<User>> GetUsersWithPermissionAsync(List<string> ids, bool isAny);
    }
}
