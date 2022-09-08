namespace Collo.Cloud.Services.Libraries.Shared.PerformanceMetrics
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPerformanceMetricsService
    {
        void Add(PerformanceMetrics metric);
        List<PerformanceMetrics> GetAll();
    }
}
