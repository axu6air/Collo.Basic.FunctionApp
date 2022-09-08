using System.ComponentModel.DataAnnotations;

namespace Collo.Cloud.IAM.Api.Models.Organization
{
    public class CreateOrgRequestModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public OrganizationSetting Settings { get; set; } = new();
    }
}
