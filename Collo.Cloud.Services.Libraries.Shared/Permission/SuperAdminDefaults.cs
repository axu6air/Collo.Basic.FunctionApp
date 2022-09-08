namespace Collo.Cloud.Services.Libraries.Shared.Permission
{
    internal static class SuperAdminDefaults
    {
        public static string Schema { get; set; } = "dummy";

        public static List<string> Assignments { get; set; } = new(){
            "iam/admin::/iam/service/organizations/any/users",
            "iam/admin::/iam/service/organizations/any/users/any",
            "iam/admin::/iam/service/organizations/any/sites",
            "iam/admin::/iam/service/organizations/any/sites/any",
            "iam/admin::/iam/service/organizations/any/users/any/assignments",
            "iam/admin::/iam/service/organizations/any/users/any/assignments/any",
            "iam/admin::/iam/service/organizations/users/permissions",
            "iam/admin::/iam/service/organizations/any/users/any/roles",
            "iam/admin::/iam/service/organizations",
            "iam/admin::/iam/service/organizations/any",
            "iam/admin::/iam/service/organizations/any/sites/any/gateways",
            "iam/admin::/iam/service/organizations/any/sites/any/gateways/any",
            "iam/admin::/iam/service/instruments",
            "iam/admin::/iam/service/instruments/any",
            "iam/admin::/iam/service/organizations/any/sites/any/gateways/any/instruments",
            "iam/admin::/iam/service/organizations/any/sites/any/gateways/any/instruments/any",
            "iam/admin::/iam/service/organizations/any/roles",
            "iam/admin::/iam/service/organizations/any/roles/any"
        };

        public static List<string> Roles { get; set; } = new(){
            "iam/user:rl",
            "iam/admin:crudl"
        };
    }
}
