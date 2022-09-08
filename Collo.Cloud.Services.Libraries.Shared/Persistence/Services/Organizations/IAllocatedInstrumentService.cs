using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Organization;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Organizations
{
    public interface IAllocatedInstrumentService
    {
        Task AllocateInstrument(List<AllocatedInstrument> allocatedInstruments);
        Task AllocateInstrument(AllocatedInstrument allocatedInstrument);
        Task DeleteInstrument(string id);
        Task<List<AllocatedInstrument>> GetAllocatedInstruments();
        Task<List<AllocatedInstrument>> GetAllocatedInstrumentWithPermissionAsync(List<string> ids, bool isAny);
    }
}
