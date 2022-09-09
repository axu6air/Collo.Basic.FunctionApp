//using Collo.Cloud.Services.Libraries.Shared.B2C;
//using Collo.Cloud.Services.Libraries.Shared.Middlewares;
//using Collo.Cloud.Services.Libraries.Shared.Permission;
//using Collo.Cloud.Services.Libraries.Shared.Persistence.Data;
//using Collo.Cloud.Services.Libraries.Shared.Persistence.Services;
//using Collo.Cloud.Services.Libraries.Shared.Seeding;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;

//var host = new HostBuilder()
//    .ConfigureFunctionsWorkerDefaults((builder) =>
//    {
//        builder.UseMiddleware<GlobalExceptionMiddleware>();
//        //builder.UseMiddleware<AuthenticationMiddleware>();
//        //builder.UseMiddleware<AuthorizationMiddleware>();
//    })
//    .ConfigureHostConfiguration(configuration =>
//    {
//        configuration.AddJsonFile("local.settings.json", optional: true);
//        //configuration.AddKeyVaultService("KEYVAULTCONFIGURATION_KV");
//    })
//    .ConfigureServices((services) =>
//    {
//        services.AddDataConfigurationService("COSMOSDBCONNECTIONSETTINGS_COMMON_KV");

//        services.AddB2CConfigurationService("AZUREADB2CSETTINGS_KV");

//        services.AddDomainConfigurationService();

//        services.AddColloPermission();

//        services.AddSeedingData("SEEDING_DATA_COMMON_KV");
//    })
//    .ConfigureLogging((logging) =>
//    {
//        logging.AddConsole();
//        logging.AddDebug();
//    })
//    .Build();

//host.Run();

using Collo.Cloud.Services.Libraries.Shared.Middlewares;
using Collo.Cloud.Services.Libraries.Shared.Permission;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Data;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults((builder) =>
    {
        builder.UseMiddleware<GlobalExceptionMiddleware>();
        //builder.UseMiddleware<AuthenticationMiddleware>();
        //builder.UseMiddleware<AuthorizationMiddleware>();
    })
    .ConfigureHostConfiguration(configuration =>
    {
        configuration.AddJsonFile("local.settings.json", optional: true);
    })
    .ConfigureServices((services) =>
    {
        services.AddDataConfigurationService("COSMOSDBCONNECTIONSETTINGS_COMMON_KV");

        services.AddDomainConfigurationService();

        services.AddColloPermission();
    })
    .ConfigureLogging((logging) =>
    {
        logging.AddConsole();
        logging.AddDebug();
    })
    .Build();

host.Run();

