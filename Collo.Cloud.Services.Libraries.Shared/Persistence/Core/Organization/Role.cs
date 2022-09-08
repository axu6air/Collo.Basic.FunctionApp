using Collo.Cloud.Services.Libraries.Shared.Helpers;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization
{
    public class Role
    {
        public Role()
        {
            Id = IdHelper.NewNanoId().ToString();
            IsDeleted = false;
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool IsDeleted { get; set; }
        public List<char> Permissions { get; set; }
    }
}
