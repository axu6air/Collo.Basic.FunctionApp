namespace Collo.Cloud.Services.Libraries.Shared.Analytics.Configuration
{
    public interface IAnalyticsServiceConfiguration
    {
        string TimeboxLength { get; set; }
        string MarginOfError { get; set; }
        string BucketResolution { get; set; }
        string AggregateResolution { get; set; }
        string AdvanceBucketCreationTime { get; set; }
    }
}
