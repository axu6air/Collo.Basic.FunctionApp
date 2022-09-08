using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization;

namespace Collo.Cloud.Services.Libraries.Shared.Seeding
{
    internal partial class SeedingDataConfiguration
    {
        internal Organization Organization
        {
            get
            {
                return new Organization
                {
                    Name = OrganizationName
                };
            }
        }

        internal User User
        {
            get
            {
                return new User()
                {
                    DisplayName = UserDisplayName,
                    Email = UserEmail,
                };
            }
        }

        public string OrganizationName { get; set; }
        public string UserPassword { get; set; }
        public string UserEmail { get; set; }
        public string UserDisplayName { get; set; }
    }
}
