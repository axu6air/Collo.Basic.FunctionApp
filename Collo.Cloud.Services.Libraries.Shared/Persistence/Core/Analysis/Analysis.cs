using Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Entities;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Analysis
{
    public class Analysis : ContainerEntity
    {
        public string Version { get; set; }
        public string DeviceId { get; set; }
        public string GatewayId { get; set; }
        public bool IsTooling { get; set; }
        public string Location { get; set; }
        public double?[] Measurements { get; set; } = default;
        public string MeasurementsModelVersion { get; set; }
        public object History { get; set; }
    }
}
