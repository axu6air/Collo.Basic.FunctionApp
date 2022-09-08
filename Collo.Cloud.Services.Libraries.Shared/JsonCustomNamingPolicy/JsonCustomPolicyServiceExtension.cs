using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace Collo.Cloud.Services.Libraries.Shared.JsonCustomPolicy
{
    public static class JsonCustomNamingPolicyServiceExtension
    {
        public static IServiceCollection AddJsonCustomNamingPolicy(this IServiceCollection services, bool isCamelCasePolicy = true)
        {
            var jsonSetting = new JsonSerializerOptions();
            if (isCamelCasePolicy)
            {
                services.Configure<JsonSerializerOptions>(jsonSetting =>
                {
                    jsonSetting.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    jsonSetting.PropertyNameCaseInsensitive = false;
                });
            }

            return services;

        }
    }
}
