using System.ComponentModel.DataAnnotations;

namespace Collo.Cloud.IAM.Api.Models.User
{
    public class UpdateRoleRequestModel
    {
        public UpdateRoleRequestModel()
        {
            Roles = new();
        }

        [Required]
        public List<string> Roles { get; set; }
    }
}
