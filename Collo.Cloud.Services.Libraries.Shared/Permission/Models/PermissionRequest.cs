using Collo.Cloud.Services.Libraries.Shared.Infrastructures.Constants;

namespace Collo.Cloud.Services.Libraries.Shared.Permission.Models
{
    public class PermissionRequest
    {
        public string Id { get; set; }
        public string Schema { get; set; }
        public string Organization { get; set; }
        public List<string> Roles { get; set; } = new();
        public List<string> Assignments { get; set; } = new();
        public DateTime CreatedAtUtc { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public DateTime DeletedAtUtc { get; set; }
        public string UpdatedBy { get; set; }
        public string DeletedBy { get; set; }
        public string Roleassignmentseparator = AssignmentConstants.ROLE_ASSIGNMENT_SEPARATOR;
        public char Rolepermissionseparator = AssignmentConstants.ROLE_PERMISSION_SEPARATOR;
        public string Idseparator = AssignmentConstants.ID_SEPARATOR;
    }
}
