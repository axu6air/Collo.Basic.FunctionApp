using Collo.Cloud.Services.Libraries.Shared.Permission.Models;

namespace Collo.Cloud.IAM.Api.Models.Assignment
{
    public class RolePermissionModel
    {
        public string ServiceName { get; set; }
        public List<Permission> Permissions { get; set; }
    }
}
