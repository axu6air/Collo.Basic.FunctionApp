namespace Collo.Cloud.IAM.Api.Models.Request
{
    public class CreateAssignmentRequestModel
    {
        public CreateAssignmentRequestModel()
        {
            Assignments = new List<string>();
            Roles = new List<string>();
        }
        public string Schema { get; set; }
        public List<string> Roles { get; set; }
        public List<string> Assignments { get; set; }
    }
}
