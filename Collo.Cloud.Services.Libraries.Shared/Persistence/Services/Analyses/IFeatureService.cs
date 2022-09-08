using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Analysis;
using System.Net;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Analyses
{
    public interface IFeatureService
    {
        Task<(Feature, HttpStatusCode)> CreateFeatureAsync(Feature feature);
    }
}
