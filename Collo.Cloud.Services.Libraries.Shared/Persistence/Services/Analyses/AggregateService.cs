using Collo.Cloud.Services.Libraries.Shared.Persistence.Core.Analysis;
using Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Repositories;
using System.Net;

namespace Collo.Cloud.Services.Libraries.Shared.Persistence.Services.Analyses
{
    public class AggregateService : IAggregateService
    {
        private readonly IRepository<Aggregate> _aggregateRepository;

        public AggregateService(IRepository<Aggregate> aggregateRepository)
        {
            _aggregateRepository = aggregateRepository;
        }

        /// <summary>
        /// Create New Aggregate Document
        /// </summary>
        /// <param name="aggregate"></param>
        /// <returns>Created Aggregate</returns>
        public async Task<Aggregate> CreateAggregate(Aggregate aggregate)
        {
            var (createdAggregate, httpStatusCode) = await _aggregateRepository.CreateAsync(aggregate);

            if (httpStatusCode == HttpStatusCode.OK) 
                return createdAggregate;

            return null;
        }

        /// <summary>
        /// Try to replace exisiting Aggregate document,
        /// if not found, create new one
        /// </summary>
        /// <param name="aggregate"></param>
        /// <returns>Created </returns>
        public async Task<Aggregate> ReplaceAggregate(Aggregate aggregate)
        {
            var (exists, _) = await _aggregateRepository.ExistsAsync(aggregate.Id);

            if (exists)
            {
                var (replacedAggregate, replaceHttpStatusCode) = await _aggregateRepository.ReplaceAsync(aggregate);

                if (replaceHttpStatusCode != HttpStatusCode.OK) 
                    return replacedAggregate;
            }
            else
            {
                var (createdAggregate, createHttpStatusCode) = await _aggregateRepository.CreateAsync(aggregate);

                if (createHttpStatusCode != HttpStatusCode.OK)
                    return createdAggregate;
            }

            return null;
        }
    }
}
