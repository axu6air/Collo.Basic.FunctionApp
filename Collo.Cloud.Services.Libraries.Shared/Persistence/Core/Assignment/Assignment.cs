using Collo.Cloud.Services.Libraries.Shared.Infrastructures.Constants;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Entities;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Assignment
{
    public class Assignment : ContainerEntity
    {
        public string Schema { get; set; }
        public string Organization { get; set; }
        public List<string> Roles { get; set; } = new();
        public List<string> Assignments { get; set; } = new();
        public string RoleAssignmentSeparator = AssignmentConstants.ROLE_ASSIGNMENT_SEPARATOR;
        public char RolePermissionSeparator = AssignmentConstants.ROLE_PERMISSION_SEPARATOR;
        public string IdSeparator = AssignmentConstants.ID_SEPARATOR;
    }
}
