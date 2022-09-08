using Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Entities;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Analysis
{
    public class Bucket : ContainerEntity
    {
        public string Schema { get; set; }
        public string PartitionKey { get; set; } //<instrumentId>::<feature>::<splitter>
        public string InstrumentId { get; set; } //InstrumentId/DeviceId
        protected override string GetPartitionKeyValue() => PartitionKey; //Override default PartitionKey 'Id'
        public bool IsTooling { get; set; }
        public string Feature { get; set; }
        public int Resolution { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Count { get; set; }
        public int?[] Times { get; set; }
        public float?[] Values { get; set; }
    }
}
