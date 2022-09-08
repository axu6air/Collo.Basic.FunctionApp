using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Analysis;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Repositories;
using System.Net;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Analyses
{
    public class FeatureService : IFeatureService
    {
        private readonly IRepository<Feature> _featureRepository;

        public FeatureService(IRepository<Feature> featureRepository)
        {
            _featureRepository = featureRepository;
        }

        /// <summary>
        /// Create Feature in Cosmos DB
        /// </summary>
        /// <param name="feature">Feature to create (insert to database)</param>
        /// <returns>Created Feature object and HTTP Response Code</returns>
        public async Task<(Feature, HttpStatusCode)> CreateFeatureAsync(Feature feature)
        {
            var (createdFeature, httpStatusCode) = await _featureRepository.CreateAsync(feature);

            return (createdFeature, httpStatusCode);
        }
    }
}
