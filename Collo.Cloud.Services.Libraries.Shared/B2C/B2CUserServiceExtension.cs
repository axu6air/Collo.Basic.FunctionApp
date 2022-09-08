using Azure.Identity;
using Collo.Cloud.Services.Libraries.Shared.B2C.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Graph;

namespace Collo.Cloud.Services.Libraries.Shared.B2C
{
    public static class B2cUserServiceExtension
    {
        /// <summary>
        /// Used for injecting Azure AD B2C service
        /// </summary>
        /// <param name="key">is Environment Variable Name</param>
        /// <returns></returns>
        public static IServiceCollection AddB2CConfigurationService(this IServiceCollection service, string key)
        {
            //Get Value from Environment
            var b2cConfig = Helpers.EnvironmentVariable<B2cUserServiceConfiguration>.GetServiceConfiguration(key);

            //Configure Service Using Values from Pervious Step
            service.Configure<B2cUserServiceConfiguration>(x =>
            {
                x.AppId = b2cConfig.AppId;
                x.TenantId = b2cConfig.TenantId;
                x.B2cExtensionAppClientId = b2cConfig.B2cExtensionAppClientId;
                x.Issuer = b2cConfig.Issuer;
                x.ClientSecret = b2cConfig.ClientSecret;
                x.Scopes = b2cConfig.Scopes;
                x.InitialPassword = b2cConfig.InitialPassword;
            });

            //Validations and Dependency Injection
            var serviceProvider = service.BuildServiceProvider();
            var b2cServiceConfiguration = serviceProvider.GetRequiredService<IOptions<B2cUserServiceConfiguration>>().Value;
            service.AddSingleton<IB2cUserServiceConfiguration>(b2cServiceConfiguration);

            ClientSecretCredential authenticationProvider = new(
                    b2cServiceConfiguration.TenantId,
                    b2cServiceConfiguration.AppId,
                    b2cServiceConfiguration.ClientSecret
                );

            service.AddTransient(x => new GraphServiceClient(authenticationProvider, b2cServiceConfiguration.Scopes));
            service.AddTransient<IB2cUserService, B2cUserService>();

            return service;
        }
    }
}
