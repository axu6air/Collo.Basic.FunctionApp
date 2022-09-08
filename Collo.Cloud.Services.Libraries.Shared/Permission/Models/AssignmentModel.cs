namespace Collo.Cloud.Services.Libraries.Shared.Permission.Models
{
    public class AssignmentModel
    {
        public string ServiceName { get; set; }
        public string RoleName { get; set; }
        public AssignmentAccessModel Organization { get; set; }
        public AssignmentAccessModel Site { get; set; }
        public AssignmentAccessModel Gateway { get; set; }
        public AssignmentAccessModel Instrument { get; set; }
        public AssignmentAccessModel User { get; set; }
    }

    public class AssignmentAccessModel
    {
        public string Id { get; set; }
        public bool AccessAny { get; set; }
    }
}
