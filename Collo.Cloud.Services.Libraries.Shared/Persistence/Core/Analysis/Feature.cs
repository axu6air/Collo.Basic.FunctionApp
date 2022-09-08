using Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Entities;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Analysis
{
    public class Feature : ContainerEntity
    {
        public string Schema { get; set; } = "http://json-schema.org/draft-04/schema#";
        public string DeliveryId { get; set; }
        public string InstrumentId { get; set; }
        public string Location { get; set; }
        public bool IsTooling { get; set; }
        public int Resolution { get; set; }
        public string Name { get; set; } //Feature Name, example: "collo/features/civ/raw"
        public float Value { get; set; }
        public DateTime MeasuredAtUtc { get; set; }
    }
}
