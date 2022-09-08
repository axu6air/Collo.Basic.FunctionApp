namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Entities
{
    public interface IEntity
    {
        string Id { get; set; }
        string CreatedBy { get; set; }
        string UpdatedBy { get; set; }
        string DeletedBy { get; set; }
        DateTime CreatedAtUtc { get; set; }
        DateTime UpdatedAtUtc { get; set; }
        DateTime DeletedAtUtc { get; set; }
    }
}