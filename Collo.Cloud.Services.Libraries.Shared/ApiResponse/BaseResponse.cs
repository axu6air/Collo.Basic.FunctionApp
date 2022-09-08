using System.Net;

namespace Collo.Cloud.Services.Libraries.Shared.ApiResponse
{
    public class BaseResponse
    {
        public HttpStatusCode StatusCode { get; set; }//200
        public string Status { get; set; }//Success,Failed
        public string Message { get; set; }//"Successful",Failed
        public object Data { get; set; } = null;
        public List<ValidationError> Errors { get; set; } = null;
    }

}
