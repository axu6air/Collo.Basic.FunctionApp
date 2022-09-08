using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Analysis;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Analyses
{
    public interface IAnalysisService
    {
        ValueTask CreateAnalysisAsync(Analysis analysis);
        ValueTask CreateAnalysisAsync(List<Analysis> analysis);
    }
}
