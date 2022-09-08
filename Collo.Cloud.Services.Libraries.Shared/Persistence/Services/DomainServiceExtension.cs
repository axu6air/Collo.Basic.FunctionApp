using Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Analyses;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Instrument;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Organizations;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Services.ServiceFactories;
using Microsoft.Extensions.DependencyInjection;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Services
{
    public static class DomainServiceExtension
    {
        public static IServiceCollection AddDomainConfigurationService(this IServiceCollection service)
        {
            service.AddScoped<ServiceFactory>();
            service.AddScoped<OrganizationServiceFactory>();
            service.AddScoped<IOrganizationService, OrganizationService>();
            service.AddScoped<IUserService, UserService>();
            service.AddScoped<ISiteService, SiteService>();
            service.AddScoped<IGatewayService, GatewayService>();
            service.AddScoped<IInstrumentService, InstrumentService>();
            service.AddTransient<IAnalysisService, AnalysisService>();
            service.AddTransient<IBucketService, BucketService>();
            service.AddTransient<IAggregateService, AggregateService>();
            service.AddTransient<IFeatureService, FeatureService>();
            service.AddTransient<IRoleService, RoleService>();

            return service;
        }

    }
}
