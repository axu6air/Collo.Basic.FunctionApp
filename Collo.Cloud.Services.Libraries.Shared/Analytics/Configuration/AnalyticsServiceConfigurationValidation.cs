using Microsoft.Extensions.Options;

namespace Collo.Cloud.Services.Libraries.Shared.Analytics.Configuration
{
    public class AnalyticsServiceConfigurationValidation : IValidateOptions<AnalyticsServiceConfiguration>
    {
        public ValidateOptionsResult Validate(string name, AnalyticsServiceConfiguration options)
        {
            if (string.IsNullOrEmpty(options.TimeboxLength))
                return ValidateOptionsResult.Fail($"{nameof(options.TimeboxLength)} configuration parameter for Timebox is required");

            if (string.IsNullOrEmpty(options.MarginOfError))
                return ValidateOptionsResult.Fail($"{nameof(options.MarginOfError)} configuration parameter for MarginOfError is required");

            if (string.IsNullOrEmpty(options.BucketResolution))
                return ValidateOptionsResult.Fail($"{nameof(options.BucketResolution)} configuration parameter for BucketResolution is required");

            if (string.IsNullOrEmpty(options.AggregateResolution))
                return ValidateOptionsResult.Fail($"{nameof(options.AggregateResolution)} configuration parameter for AggregateResolution is required");

            if (string.IsNullOrEmpty(options.AdvanceBucketCreationTime))
                return ValidateOptionsResult.Fail($"{nameof(options.AdvanceBucketCreationTime)} configuration parameter for AdvanceBucketCreationTime is required");

            return ValidateOptionsResult.Success;
        }
    }
}
