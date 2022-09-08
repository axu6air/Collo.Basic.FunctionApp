namespace Collo.Cloud.Services.Libraries.Shared.PerformanceMetrics
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T">payload</typeparam>
    public class BaseMetrics<T> where T : class
    {
        //add more common property if need;
        public string name { get; set; }
        public T Payload { get; set; }
    }

}
