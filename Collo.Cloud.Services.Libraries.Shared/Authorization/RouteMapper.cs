namespace Collo.Cloud.Services.Libraries.Shared.Authorization
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public class RouteMapperAttribute : Attribute
    {
        public string RouteName { get; set; }
        public string RoleName { get; set; }

        public RouteMapperAttribute(string routeName, string roleName)
        {
            RouteName = routeName;
            RoleName = roleName;
        }
    }
}
