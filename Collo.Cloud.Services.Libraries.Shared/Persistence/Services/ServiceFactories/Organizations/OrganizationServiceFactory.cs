using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Organizations;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Services.ServiceFactories
{
    public class OrganizationServiceFactory
    {
        private readonly IOrganizationService _organizationService;
        private readonly string _id;

        public Organization Organization { get; set; } = new();

        public OrganizationServiceFactory(IOrganizationService organizationService, string id)
        {
            _organizationService = organizationService;
            _id = id;

            InitializeOrganization();
        }

        private void InitializeOrganization()
        {
            Organization = _organizationService.GetOrganizationAsync(_id).GetAwaiter().GetResult();
        }

    }
}
