using Microsoft.Extensions.Options;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Configurations
{
    public class CosmosDbDataServiceConfigurationValidation : IValidateOptions<CosmosDbDataServiceConfiguration>
    {
        public ValidateOptionsResult Validate(string name, CosmosDbDataServiceConfiguration options)
        {

            if (string.IsNullOrEmpty(options.ConnectionString))
                return ValidateOptionsResult.Fail($"{nameof(options.ConnectionString)} Configuration parameter for the Azure Cosmos Db connection String is required");
            if (string.IsNullOrEmpty(options.DatabaseName))
                return ValidateOptionsResult.Fail($"{nameof(options.DatabaseName)} Configuration parameter for the Azure Cosmos Db database Name is required");

            if (options.Containers.Count == 0)
                return ValidateOptionsResult.Fail("Configuration parameter for at least one container is required");

            if (!ValidateBudgetInput(options))
                return ValidateOptionsResult.Fail("Configuration parameter for Database budget is required");

            if (!ValidateTimeToLiveInput(options))
                return ValidateOptionsResult.Fail("Configuration parameter for TimeToLive for each container is required or invalid (integers >0 or -1)");

            if (!ValidatePartitionKeyInput(options))
                return ValidateOptionsResult.Fail("Configuration parameter for all container's partition key path is required");

            if (string.IsNullOrEmpty(options.MaxRetryAttempts))
                return ValidateOptionsResult.Fail($"{nameof(options.MaxRetryAttempts)} Configuration parameter for number of retries upon reaching rate limit is required");
            if (string.IsNullOrEmpty(options.MaxRetryWaitTime))
                return ValidateOptionsResult.Fail($"{nameof(options.MaxRetryWaitTime)} Configuration parameter for retry wait time upon reaching rate limit is required");

            return ValidateOptionsResult.Success;
        }

        private bool ValidateBudgetInput(CosmosDbDataServiceConfiguration options)
        {
            if (string.IsNullOrEmpty(options.Budget) || !int.TryParse(options.Budget, out int _))
            {
                foreach (var container in options.Containers)
                {
                    if (string.IsNullOrEmpty(container.Budget) || !int.TryParse(container.Budget, out int _))
                        return false;
                }
            }

            return true;
        }

        private bool ValidateTimeToLiveInput(CosmosDbDataServiceConfiguration options)
        {
            foreach (var container in options.Containers)
            {
                if (string.IsNullOrEmpty(container.TimeToLive) || !int.TryParse(container.TimeToLive, out int containerTimeToLive))
                    return false;
                if (containerTimeToLive < -1 || containerTimeToLive == 0) return false;
            }

            return true;
        }

        private bool ValidatePartitionKeyInput(CosmosDbDataServiceConfiguration options)
        {
            return options.Containers.Any(x => !string.IsNullOrEmpty(x.PartitionKeyPath));

        }

    }
}
