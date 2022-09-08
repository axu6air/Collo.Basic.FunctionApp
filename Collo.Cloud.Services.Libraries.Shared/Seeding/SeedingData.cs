using Collo.Cloud.Services.Libraries.Shared.B2C;
using Collo.Cloud.Services.Libraries.Shared.B2C.Configurations;
using Collo.Cloud.Services.Libraries.Shared.Helpers;
using Collo.Cloud.Services.Libraries.Shared.Permission;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Repositories;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Services;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Organizations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using CosmosAssignment = Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Assignment;
using CosmosOrganization = Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization;

namespace Collo.Cloud.Services.Libraries.Shared.Seeding
{
    public static class SeedingDataCollo
    {
        internal static CosmosOrganization.Organization Organization { get; set; }
        internal static CosmosOrganization.User User { get; set; }

        public static async void AddSeedingData(this IServiceCollection services, string key)
        {
            var seedingDataConfiguration = EnvironmentVariable<SeedingDataConfiguration>.GetServiceConfiguration(key);
            Organization = seedingDataConfiguration.Organization;
            User = seedingDataConfiguration.User;

            var organizationService = services.BuildServiceProvider().GetRequiredService<IRepository<CosmosOrganization.Organization>>();
            var assignmentService = services.BuildServiceProvider().GetRequiredService<IRepository<CosmosAssignment.Assignment>>();
            var b2cUserService = services.BuildServiceProvider().GetRequiredService<IB2cUserService>();
            var b2cUserServiceConfiguration = services.BuildServiceProvider().GetRequiredService<IB2cUserServiceConfiguration>();

            //Create Organization if Doesn't Exist
            var organizationExists = await OrganizationExistsByName(organizationService);
            if (!organizationExists)
            {
                _ = await organizationService.CreateAsync(new CosmosOrganization.Organization
                {
                    Id = Organization.Id,
                    Name = Organization.Name,
                    Allocations = Organization.Allocations,
                    Users = Organization.Users,
                    Roles = Organization.Roles,
                    Sites = Organization.Sites
                });
            }

            //Create User if Doesn't Exist and 
            var userService = services.BuildServiceProvider().GetRequiredService<ServiceFactory>().GetUserService(Organization.Id);

            if (!UserExistsByEmail(userService))
            {
                //Create Cosmos User
                await userService.CreateUserAsync(new CosmosOrganization.User
                {
                    Id = User.Id,
                    DisplayName = User.DisplayName,
                    Email = User.Email,
                    LocalUserPrincipalName = User.LocalUserPrincipalName
                });
                //Create Assignment
                await assignmentService.CreateAsync(new CosmosAssignment.Assignment
                {
                    Id = $"{Organization.Id}::{User.Id}::1",
                    Schema = SuperAdminDefaults.Schema,
                    Organization = Organization.Id,
                    Roles = SuperAdminDefaults.Roles,
                    Assignments = SuperAdminDefaults.Assignments
                });
            }

            if (!await b2cUserService.UserExistsByEmail(User.Email))
            {
                var graphUser = PrepareB2cUser(b2cUserServiceConfiguration, seedingDataConfiguration);
                User createdB2cUser = await b2cUserService.CreateAsync(graphUser);

                if (createdB2cUser != null)
                {
                    User.IdB2c = createdB2cUser?.Id;
                    await userService.UpdateUser(User);
                }
            }
        }

        public static async Task<bool> OrganizationExistsByName(IRepository<CosmosOrganization.Organization> organizationService)
        {
            var organization = await organizationService.GetAsync(x => x.Name == Organization.Name);

            if (organization.results.Count != 0)
            {
                Organization.Id = organization.results.FirstOrDefault().Id;
                return true;
            }

            return false;
        }

        private static bool UserExistsByEmail(IUserService userService)
        {
            var user = userService.GetUsersAsync().Result.Find(x => x.Email == User.Email);

            if (user != null)
            {
                User.Id = user.Id;
                return true;
            }

            return false;
        }

        private static User PrepareB2cUser(IB2cUserServiceConfiguration b2CUserServiceConfiguration, SeedingDataConfiguration seedingDataConfiguration)
        {
            var additionalData = new Dictionary<string, object>();
            additionalData.Add($"extension_{b2CUserServiceConfiguration.B2cExtensionAppClientId.Replace("-", "")}_OrgId", Organization.Id);
            additionalData.Add($"extension_{b2CUserServiceConfiguration.B2cExtensionAppClientId.Replace("-", "")}_UserId", User.Id);

            var graphUser = new User
            {
                AccountEnabled = true,
                DisplayName = User.DisplayName,
                OtherMails = new List<string> { User.Email },
                Mail = User.Email,
                UserPrincipalName = $"{Guid.NewGuid()}@{b2CUserServiceConfiguration.Issuer}",
                Identities = new List<ObjectIdentity>
                    {
                        new ObjectIdentity{ SignInType = "emailAddress", Issuer = b2CUserServiceConfiguration.Issuer, IssuerAssignedId = User.Email }
                    },
                AdditionalData = additionalData,
                UserType = "Member",
                CreationType = "LocalAccount",
                GivenName = User.DisplayName,
                Surname = User.DisplayName,
                PasswordProfile = new PasswordProfile { ForceChangePasswordNextSignIn = false, ForceChangePasswordNextSignInWithMfa = false, Password = seedingDataConfiguration.UserPassword },
            };

            return graphUser;
        }
    }
}
