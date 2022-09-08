using Newtonsoft.Json;

namespace Collo.Cloud.Services.Libraries.Shared.Helpers
{
    /// <summary>
    /// Deserialize and get service configuration model from Environment Variables
    /// </summary>
    /// <typeparam name="T">is the configuration model for the service to deserialize</typeparam>
    public class EnvironmentVariable<T>
    {
        private static string GetEnvironmentVariable(string key)
        {
            var jsonString = Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process);
            if (!string.IsNullOrEmpty(jsonString))
                return jsonString;

            throw new Exception("Environment variable not found for " + key);
        }

        public static T GetServiceConfiguration(string key)
        {
            var configuration = GetEnvironmentVariable(key);

            return JsonConvert.DeserializeObject<T>(configuration);
        }

    }
}
