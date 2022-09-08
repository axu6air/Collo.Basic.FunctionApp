namespace Collo.Cloud.Services.Libraries.Shared.ApiResponse
{
    public class ValidationError
    {
        public string PropertyName { get; set; }
        public string[] ErrorList { get; set; }
    }

}
