namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Entities
{
    public interface IContainerEntity : IEntity
    {
        string Type { get; set; }

        string PartitionKey { get; }


    }
}
