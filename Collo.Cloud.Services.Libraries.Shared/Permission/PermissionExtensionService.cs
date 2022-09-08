using Collo.Cloud.Services.Libraries.Shared.Permission.Interfaces;
using Collo.Cloud.Services.Libraries.Shared.Permission.PermissionTree;
using Microsoft.Extensions.DependencyInjection;

namespace Collo.Cloud.Services.Libraries.Shared.Permission
{
    public static class PermissionExtensionService
    {
        public static IServiceCollection AddColloPermission(this IServiceCollection services)
        {
            services.AddTransient<IColloPermissionService, ColloPermissionService>();
            services.AddTransient<PermissionAssignmentsService>();
            services.AddScoped<TreeStructureFactory>();
            return services;
        }
    }
}
