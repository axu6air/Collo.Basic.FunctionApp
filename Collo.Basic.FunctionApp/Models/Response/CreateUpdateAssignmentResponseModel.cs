namespace Collo.Cloud.IAM.Api.Models.Response
{
    public class CreateUpdateAssignmentResponseModel
    {
        public CreateUpdateAssignmentResponseModel()
        {
            Assignments = new List<string>();
            Roles = new List<string>();
        }
        public string Id { get; set; }
        public string Schema { get; set; }
        public string Organization { get; set; }
        public List<string> Roles { get; set; }
        public List<string> Assignments { get; set; }
    }
}
