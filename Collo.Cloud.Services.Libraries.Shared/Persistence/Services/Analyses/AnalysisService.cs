using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Analysis;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Repositories;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Analyses
{
    public class AnalysisService : IAnalysisService
    {
        private readonly IRepository<Analysis> _analysisRepository;
        public AnalysisService(IRepository<Analysis> analysisRepository)
        {
            _analysisRepository = analysisRepository;
        }

        public async ValueTask CreateAnalysisAsync(Analysis analysis)
        {
            if (analysis != null)
                await _analysisRepository.CreateAsync(analysis);
        }

        public async ValueTask CreateAnalysisAsync(List<Analysis> analyses)
        {
            if (analyses != null && analyses.Count > 0)
                await _analysisRepository.CreateAsync(analyses);
        }
    }
}
