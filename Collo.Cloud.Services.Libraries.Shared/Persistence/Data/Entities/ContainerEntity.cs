using Collo.Cloud.Services.Libraries.Shared.Helpers;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Entities
{
    public abstract class ContainerEntity : IContainerEntity
    {
        public virtual string Id { get; set; } = IdHelper.NewNanoId().ToString();

        public string Type { get; set; }

        string IContainerEntity.PartitionKey => GetPartitionKeyValue();

        public ContainerEntity() => Type = GetType().Name;

        protected virtual string GetPartitionKeyValue() => Id;

        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string DeletedBy { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public DateTime DeletedAtUtc { get; set; }
        public int Ttl { get; set; } = -1;
    }
}
