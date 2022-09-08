using Collo.Cloud.Services.Libraries.Shared.Commons.Enums;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Entities;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Instrument
{
    public class Instrument : ContainerEntity, ISoftDeleteEnabledEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } = InstrumentStatus.Unallocated.ToString();
        public bool IsDeleted { get; set; } = default;

        public AllocatedTo AllocatedTo { get; set; } = default;
    }

    public class AllocatedTo
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
