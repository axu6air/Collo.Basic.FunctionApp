using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization;
using UserEntity = Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization.User;

namespace Collo.Cloud.IAM.Api.Models.Organization
{
    public class GetOrgResponseModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Allocations { get; set; } = new();
        public List<UserEntity> Users { get; set; } = new();
        public List<Role> Roles { get; set; } = new();
        public List<Site> Sites { get; set; } = new();
        public List<AllocatedInstrument> AllocatedInstruments { get; set; } = new();
        public OrganizationSetting Settings { get; set; }

        public bool IsDeleted { get; set; }
    }
}
