namespace Collo.Cloud.Services.Libraries.Shared.PerformanceMetrics
{
    /// <summary>
    ///
    /// </summary>
    public class PerformanceMetricsService : IPerformanceMetricsService
    {
        private readonly int deadlineMs;// default 1 mintues
        private DateTime startdate;
        private readonly List<PerformanceMetrics> metricsList = new List<PerformanceMetrics>();
        public bool IsExpired => (DateTime.Now - startdate).TotalMilliseconds > deadlineMs;

        public PerformanceMetricsService(int deadlineInMilliSecond)
        {
            deadlineMs = deadlineInMilliSecond;
        }

        public void Add(PerformanceMetrics metric)
        {
            if (IsExpired)
            {
                metricsList.Clear();
                startdate = DateTime.Now;
                return;
            }
            metricsList.Add(metric);
        }

        public List<PerformanceMetrics> GetAll()
        {
            return metricsList.ToList();
        }
    }
}
