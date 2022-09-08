using Microsoft.Azure.Functions.Worker.Http;
using System.ComponentModel.DataAnnotations;

namespace Collo.Cloud.Services.Libraries.Shared.ApiResponse
{
    public static class ModelValidationExtension
    {
        public static CustomValidators<T> BuildValidation<T>(T res)
        {
            var results = new List<ValidationResult>();
            Dictionary<string, List<string>> myErrors = new Dictionary<string, List<string>>();

            var body = new CustomValidators<T>();
            body.Value = res;
            body.IsValid = Validator.TryValidateObject(body.Value, new ValidationContext(body.Value, null, null), results, true);

            foreach (var s in results)
            {
                foreach (var key in s.MemberNames)
                {
                    if (myErrors.TryGetValue(key, out List<string> valueList))
                        valueList.Add(s.ErrorMessage);
                    else
                        myErrors.Add(key, new List<string> { s.ErrorMessage });
                }
            }
            body.ValidationErrors = myErrors.Select(s => new ValidationError { PropertyName = s.Key, ErrorList = s.Value.ToArray() });
            return body;
        }

        public static async Task<CustomValidators<T>> ValidateRequestAsync<T>(this HttpRequestData request)
        {
            var body = await request.ReadFromJsonAsync<T>();
            return BuildValidation<T>(body);
        }

        //public static async Task<CustomValidators<T>> GetQueryParamAsync<T>(this HttpRequestData request)
        //{
        //    string res = JsonSerializer.SerializeAsync<T>("");
        //    return BuildValidation<T>(res);
        //}
    }
}