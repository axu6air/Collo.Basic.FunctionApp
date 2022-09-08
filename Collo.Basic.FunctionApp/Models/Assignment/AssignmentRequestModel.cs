using Collo.Cloud.Services.Libraries.Shared.Permission.Models;

namespace Collo.Cloud.IAM.Api.Models.Assignment
{

    public class AssignmentRequestModel
    {
        public string ServiceName { get; set; }
        public string RoleName { get; set; }
        public AssignmentAccessModel Organization { get; set; }
        public AssignmentAccessModel Site { get; set; }
        public AssignmentAccessModel Gateway { get; set; }
        public AssignmentAccessModel Instrument { get; set; }
        public AssignmentAccessModel User { get; set; }
    }
}
