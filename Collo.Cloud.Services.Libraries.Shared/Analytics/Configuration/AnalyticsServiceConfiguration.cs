namespace Collo.Cloud.Services.Libraries.Shared.Analytics.Configuration
{
    public class AnalyticsServiceConfiguration : IAnalyticsServiceConfiguration
    {
        public string TimeboxLength { get; set; } //Milliseconds
        public string MarginOfError { get; set; } //Milliseconds
        public string BucketResolution { get; set; } //Milliseconds
        public string AggregateResolution { get; set; } //Milliseconds
        public string AdvanceBucketCreationTime { get; set; } //Milliseconds, Pre creation
    }
}