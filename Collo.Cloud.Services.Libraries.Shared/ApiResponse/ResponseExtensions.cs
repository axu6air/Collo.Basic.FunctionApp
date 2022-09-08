using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace Collo.Cloud.Services.Libraries.Shared.ApiResponse
{
    public static class ResponseExtensions
    {
        public static async Task<HttpResponseData> OkResult(
            this HttpRequestData request,
            object data = null,
            string status = "Success",
            string message = "Successful",
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var result = new BaseResponse
            {
                StatusCode = statusCode,
                Status = status,
                Message = message,
                Data = data
            };
            var response = request.CreateResponse();
            await response.WriteAsJsonAsync(result, result.StatusCode);
            return response;
        }

        public static async Task<HttpResponseData> BadRequestResult(
            this HttpRequestData request,
            object data = null,
            string status = "",
            string message = "")
        {
            var result = new BaseResponse
            {
                StatusCode = HttpStatusCode.BadRequest,
                Status = status,
                Message = message,
                Data = data
            };
            var response = request.CreateResponse();
            await response.WriteAsJsonAsync(result, result.StatusCode);
            return response;
        }

        public static async Task<HttpResponseData> ValidationResult(this HttpRequestData request, IEnumerable<ValidationError> validations)
        {
            var result = new BaseResponse
            {
                StatusCode = HttpStatusCode.BadRequest,
                Status = "ValidationError",
                Message = "Validation Fail",
                Errors = validations?.ToList()
            };

            var response = request.CreateResponse();
            await response.WriteAsJsonAsync(result, result.StatusCode);
            return response;
        }


        public static async Task<HttpResponseData> ExceptionResult(this HttpRequestData request, Exception ex, string message = null)
        {
            //we can write log here..

            var result = new BaseResponse
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Status = "Error",
                Message = ex.Message ?? message
            };
            var response = request.CreateResponse();
            await response.WriteAsJsonAsync(result, result.StatusCode);
            return response;
        }

        public static async Task<HttpResponseData> NoContentResult(this HttpRequestData request, string message = null)
        {
            //we can write log here..

            var result = new BaseResponse
            {
                StatusCode = HttpStatusCode.NoContent,
                //Status = "No Content",
                //Message = message
            };
            var response = request.CreateResponse();
            await response.WriteAsJsonAsync(result, HttpStatusCode.NoContent);
            return response;
        }
    }
}
