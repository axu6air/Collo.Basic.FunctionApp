using System.ComponentModel.DataAnnotations;

namespace Collo.Cloud.IAM.Api.Models.User
{
    public class CreateLocalUserRequestModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string DisplayName { get; set; }
        [Required]
        public string LocalUserPrincipalName { get; set; }
    }
}
