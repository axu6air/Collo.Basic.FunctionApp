using Collo.Cloud.Services.Libraries.Shared.ApiResponse;
using Collo.Cloud.Services.Libraries.Shared.Commons;
using Collo.Cloud.Services.Libraries.Shared.CustomExceptionHandlers;
using Collo.Cloud.Services.Libraries.Shared.Extensions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Collo.Cloud.Services.Libraries.Shared.Middlewares
{
    public class GlobalExceptionMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly ILogger _logger;
        public GlobalExceptionMiddleware()
        {
            _logger = LoggerCore.GetLogger<GlobalExceptionMiddleware>();
        }

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var functionName = context.GetTargetFunctionMethod().Name;

            try
            {

                _logger.LogInformation($"{functionName} is getting executed");
                await next(context);
                _logger.LogInformation($"{functionName} is executed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{functionName} threw an error");

                if (ex.InnerException is NullEntityException)
                {
                    var req = context.GetHttpRequestData();
                    context.SetHttpResponseData(await req.BadRequestResult("This data is not valid"));
                    return;
                }

                _logger.LogError($"Unexpected Error in {context.FunctionDefinition.Name}: {ex.Message}");
                context.SetHttpResponseStatusCode(HttpStatusCode.InternalServerError);
                return;
            }
        }
    }
}
