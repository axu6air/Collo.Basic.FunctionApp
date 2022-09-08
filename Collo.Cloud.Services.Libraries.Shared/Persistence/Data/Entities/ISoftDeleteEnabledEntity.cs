namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Entities
{
    public interface ISoftDeleteEnabledEntity
    {
        bool IsDeleted { get; set; }
    }
}
