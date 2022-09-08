namespace Collo.Cloud.Services.Libraries.Shared.ApiResponse
{
    public class CustomValidators<T>
    {
        public bool IsValid { get; set; }
        public T Value { get; set; }
        public IEnumerable<ValidationError> ValidationErrors { get; set; }
    }
}
