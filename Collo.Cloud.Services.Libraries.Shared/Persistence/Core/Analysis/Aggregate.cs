using Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Entities;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Analysis
{
    public class Aggregate : ContainerEntity
    {
        public string Schema = "aggregate/1.0";
        public string PartitionKey;
        protected override string GetPartitionKeyValue() => PartitionKey;
        public string InstrumentId { get; set; }
        public string SenderId { get; set; }
        public string Location { get; set; }
        public bool IsTooling { get; set; }
        public string Feature { get; set; }
        public int Resolution { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Dictionary<string, double> Values { get; set; }
    }
}
