using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Analysis;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Analyses
{
    public interface IAggregateService
    {
        Task<Aggregate> ReplaceAggregate(Aggregate aggregate);

        Task<Aggregate> CreateAggregate(Aggregate aggregate);
    }
}
