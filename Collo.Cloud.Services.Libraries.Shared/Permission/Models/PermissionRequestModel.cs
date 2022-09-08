namespace Collo.Cloud.Services.Libraries.Shared.Permission.Models
{
    public class PermissionModel
    {
        public string ServiceName { get; set; }
        public List<Permission> Permissions { get; set; }
    }

    public class Permission
    {
        public string RoleId { get; set; }
        public string Operation { get; set; }
    }
}
