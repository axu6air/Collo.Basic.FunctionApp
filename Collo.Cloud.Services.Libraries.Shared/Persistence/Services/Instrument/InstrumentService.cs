using Collo.Cloud.Services.Libraries.Shared.Commons.Enums;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Instrument;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Repositories;
using Humanizer;
using Microsoft.Azure.Cosmos;
using System.Linq.Expressions;
using InstrumentEntity = Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Instrument.Instrument;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Instrument
{
    public class InstrumentService : IInstrumentService
    {
        private readonly IRepository<InstrumentEntity> _instrumentRepository;

        public InstrumentService(IRepository<InstrumentEntity> instrumentRepository)
        {
            _instrumentRepository = instrumentRepository;
        }

        public async ValueTask<InstrumentEntity> GetAsync(string id)
        {
            var (result, _) = await _instrumentRepository.GetAsync(id);

            var a = await _instrumentRepository.PageAsync(x => x.Id != null, 3, null, true);

            return result;
        }

        public async ValueTask<List<InstrumentEntity>> GetAsync(List<string> ids)
        {
            var (results, _) = await _instrumentRepository.GetAsync(ids);

            return results;
        }

        public async ValueTask<List<InstrumentEntity>> GetAsync()
        {
            var (results, _) = await _instrumentRepository.GetByQueryAsync($"SELECT * FROM Instrument");

            return results;
        }

        public async ValueTask<List<InstrumentEntity>> GetAsync(
            Expression<Func<InstrumentEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            var (results, _) = await _instrumentRepository.GetAsync(predicate, cancellationToken);

            return results;
        }

        public async ValueTask<List<InstrumentEntity>> GetUnallocatedInstrumentsAsync()
        {
            var (result, _) = await _instrumentRepository.GetAsync(x => x.Status.Equals(InstrumentStatus.Unallocated.ToString(), StringComparison.OrdinalIgnoreCase));
            return result;
        }

        public async ValueTask<InstrumentEntity> CreateAsync(InstrumentEntity instrument)
        {
            var (result, _) = await _instrumentRepository.CreateAsync(instrument);

            return result;
        }

        public async ValueTask<InstrumentEntity> PatchAsync(string id, IReadOnlyList<PatchOperation> patchOperations)
        {
            var (result, _) = await _instrumentRepository.PatchAsync(id, patchOperations);

            return result;
        }

        public async ValueTask AllocateInstrument(string instrumentId, AllocatedTo allocatedTo)
        {
            await _instrumentRepository.PatchAsync(instrumentId,
                new[] {
                        PatchOperation.Replace($"/{nameof(InstrumentEntity.Status).Camelize()}", InstrumentStatus.Allocated.ToString()),
                        PatchOperation.Replace($"/{nameof(InstrumentEntity.AllocatedTo).Camelize()}", allocatedTo)
                });
        }

        public async ValueTask UnAllocateInstrument(string instrumentId)
        {
            await _instrumentRepository.PatchAsync(instrumentId, new[] {
                    PatchOperation.Replace($"/{nameof(InstrumentEntity.Status).Camelize()}", InstrumentStatus.Unallocated.ToString()),
                    PatchOperation.Replace<AllocatedTo>($"/{nameof(InstrumentEntity.AllocatedTo).Camelize()}",null),

                });
        }

        public async ValueTask UpdateAsync(InstrumentEntity instrument)
        {
            await _instrumentRepository.UpdateAsync(instrument);
        }

        public async ValueTask<InstrumentEntity> DeleteAsync(string instrumentId)
        {
            var (instrument, _) = await _instrumentRepository.PatchAsync(instrumentId, new[] {
                PatchOperation.Replace($"/{nameof(InstrumentEntity.IsDeleted).Camelize()}", true)
            });

            return instrument;
        }
    }
}
