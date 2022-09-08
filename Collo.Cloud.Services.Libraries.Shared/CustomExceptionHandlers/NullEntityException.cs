using Collo.Cloud.Services.Libraries.Shared.Commons;
using Microsoft.Extensions.Logging;

namespace Collo.Cloud.Services.Libraries.Shared.CustomExceptionHandlers
{
    [Serializable]
    public class NullEntityException : Exception
    {
        private readonly ILogger _logger;

        public NullEntityException(string message) : base(message)
        {
            _logger = LoggerCore.GetLogger<NullEntityException>();
            _logger.LogError(message);
        }
    }
}
