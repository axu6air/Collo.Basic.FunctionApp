namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Analysis
{
    public class Accumulate
    {
        public string Schema { get; set; } = "http://json-schema.org/draft-04/schema#";
        public string InstrumentId { get; set; }
        public string DeliveryId { get; set; }
        public string Location { get; set; }
        public bool IsTooling { get; set; }
        public int Resolution { get; set; }
        public string Feature { get; set; }
        public float Value { get; set; }
        public DateTime MeasuredAtUtc { get; set; }
    }
}
