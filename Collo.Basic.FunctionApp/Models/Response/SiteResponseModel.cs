using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization;

namespace Collo.Cloud.IAM.Api.Models.Response
{
    public class SiteResponseModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public List<Gateway> Gateways { get; set; }
    }
}
