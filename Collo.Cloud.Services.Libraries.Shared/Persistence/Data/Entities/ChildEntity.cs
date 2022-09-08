using Collo.Cloud.Services.Libraries.Shared.Helpers;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Entities
{
    public class ChildEntity : IChildEntity
    {
        public virtual string Id { get; set; } = IdHelper.NewNanoId().ToString();

        public string Type { get; set; }

        protected virtual string GetPartitionKeyValue() => Id;

        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string DeletedBy { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public DateTime DeletedAtUtc { get; set; }
    }
}