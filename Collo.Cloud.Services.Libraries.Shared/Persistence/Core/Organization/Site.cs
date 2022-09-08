using Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Entities;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization
{
    public class Site : ChildEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public string Location { get; set; }

        public List<Gateway> Gateways { get; set; } = new();
    }
}
