using Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Entities;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization
{
    public class Organization : ContainerEntity, ISoftDeleteEnabledEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public List<string> Allocations { get; set; } = new();
        public List<User> Users { get; set; } = new();
        public List<Role> Roles { get; set; } = new();
        public List<Site> Sites { get; set; } = new();

        public Setting Settings { get; set; } = new();
        public bool IsDeleted { get; set; } = false;
    }
}
