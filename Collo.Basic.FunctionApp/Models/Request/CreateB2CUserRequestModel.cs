using Collo.Cloud.Services.Libraries.Shared.Helpers;
using Microsoft.Graph;

namespace Collo.Cloud.IAM.Api.Models.Request
{
    public class CreateB2CUserRequestModel
    {
        public CreateB2CUserRequestModel()
        {
            Identities = new();
            PasswordProfile = new()
            {
                Password = B2CPasswordHelper.GenerateNewPassword(3, 3, 2)
            };
            PasswordPolicies = "DisablePasswordExpiration";
        }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string DisplayName { get; set; }
        public List<ObjectIdentity> Identities { get; set; }
        public PasswordProfile PasswordProfile { get; set; }
        public string PasswordPolicies { get; set; }
    }
}
