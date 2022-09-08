using Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Entities;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization
{
    public class User : ChildEntity
    {
        public string IdB2c { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string LocalUserPrincipalName { get; set; }
    }
}
