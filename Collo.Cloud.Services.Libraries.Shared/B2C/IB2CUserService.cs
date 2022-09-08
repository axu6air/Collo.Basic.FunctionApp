using Microsoft.Graph;

namespace Collo.Cloud.Services.Libraries.Shared.B2C
{
    public interface IB2cUserService
    {
        Task<User> CreateAsync(User user);
        Task DeleteAsync(string objectId);
        Task<User> GetUserAsync(string objectId);
        Task<User> GetbyEmailAsync(string email);
        Task<bool> UserExistsByEmail(string email);

        Task<Invitation> InviteUser(Invitation invitation);
    }
}
