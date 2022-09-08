using System.ComponentModel.DataAnnotations;

namespace Collo.Cloud.IAM.Api.Models.User
{
    public class CreateUserRequestModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string DisplayName { get; set; }
    }
}
