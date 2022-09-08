using Microsoft.Graph;

namespace Collo.Cloud.Services.Libraries.Shared.B2C
{
    public class B2cUserService : IB2cUserService
    {
        private readonly GraphServiceClient _graphServiceClient;
        public B2cUserService(GraphServiceClient graphServiceClient)
        {
            _graphServiceClient = graphServiceClient;
        }

        public async Task<User> CreateAsync(User user)
        {
            try
            {
                var result = await _graphServiceClient.Users
                .Request()
                .AddAsync(user);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task DeleteAsync(string objectid)
        {
            try
            {
                await _graphServiceClient.Users[objectid]
                   .Request()
                   .DeleteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<User> GetUserAsync(string objectId)
        {
            try
            {
                var result = await _graphServiceClient.Users[objectId].
                    Request()
                    .GetAsync();

                if (result != null)
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<User> GetbyEmailAsync(string email)
        {
            var collectionPage = await _graphServiceClient.Users
                .Request()
                .Filter($"identities/any(c:c/issuerAssignedId eq '{email}' and c/issuer eq '{email}')")
                .GetAsync();

            return collectionPage.FirstOrDefault();
        }

        public async Task<bool> UserExistsByEmail(string email)
        {
            var user = await GetbyEmailAsync(email);

            return user != null;
        }

        public async Task<Invitation> InviteUser(Invitation invitation)
        {
            return await _graphServiceClient.Invitations.Request().AddAsync(invitation);
        }

    }
}
