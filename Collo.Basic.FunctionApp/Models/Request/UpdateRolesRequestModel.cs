namespace Collo.Cloud.IAM.Api.Models.Request
{
    public class UpdateRolesRequestModel
    {
        public UpdateRolesRequestModel()
        {
            Roles = new List<string>();
        }
        public List<string> Roles { get; set; }
    }
}
