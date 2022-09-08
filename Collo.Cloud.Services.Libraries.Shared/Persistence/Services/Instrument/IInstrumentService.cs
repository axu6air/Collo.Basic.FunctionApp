using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Instrument;
using Microsoft.Azure.Cosmos;
using System.Linq.Expressions;
using InstrumentEntity = Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Instrument.Instrument;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Instrument
{
    public interface IInstrumentService
    {
        ValueTask<InstrumentEntity> GetAsync(string id);
        ValueTask<List<InstrumentEntity>> GetAsync(List<string> ids);
        ValueTask<List<InstrumentEntity>> GetAsync();
        ValueTask<List<InstrumentEntity>> GetAsync(
            Expression<Func<InstrumentEntity, bool>> predicate,
            CancellationToken cancellationToken = default);
        ValueTask<List<InstrumentEntity>> GetUnallocatedInstrumentsAsync();
        ValueTask<InstrumentEntity> CreateAsync(InstrumentEntity instrument);
        ValueTask<InstrumentEntity> PatchAsync(string id, IReadOnlyList<PatchOperation> patchOperations);
        ValueTask AllocateInstrument(string instrumentId, AllocatedTo allocatedTo);
        ValueTask UnAllocateInstrument(string instrumentId);
        ValueTask UpdateAsync(InstrumentEntity instrument);
        ValueTask<InstrumentEntity> DeleteAsync(string instrumentId);
    }
}
