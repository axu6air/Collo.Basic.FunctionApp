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
        //configuration.AddKeyVaultService("KEYVAULTCONFIGURATION_KV");
    })
    .ConfigureServices((services) =>
    {
        var logger = LoggerCore.GetLogger<Program>();

        services.AddDataConfigurationService("COSMOSDBCONNECTIONSETTINGS_COMMON_KV");

        try
        {
            logger.LogInformation("AZUREADB2CSETTINGS_KV executing");
            services.AddB2CConfigurationService("AZUREADB2CSETTINGS_KV");
            logger.LogInformation("AZUREADB2CSETTINGS_KV executed");

        }
        catch (Exception ex)
        {

            logger.LogError("AZUREADB2CSETTINGS_KV: " + ex.Message, ex);
        }

        services.AddDomainConfigurationService();

        services.AddColloPermission();

        try
        {
            logger.LogInformation("Seeding Data executing");
            services.AddSeedingData("SEEDING_DATA_COMMON_KV");
            logger.LogInformation("Seeding Data executed");

        }
        catch (Exception ex)
        {

            logger.LogError("AddSeedingData: " + ex.Message, ex);
        }



    })
    .ConfigureLogging((logging) =>
    {
        logging.AddConsole();
        logging.AddDebug();
    })
    .Build();

host.RunAsync();
