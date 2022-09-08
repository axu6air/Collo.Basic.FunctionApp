using Microsoft.Azure.Functions.Worker.Http;

namespace Collo.Cloud.Services.Libraries.Shared.Helpers
{
    public class UrlMaker
    {
        public static string GetUrl(HttpRequestData req, string registeredRoute)
        {
            var url = req.Url.ToString();
            var route = registeredRoute.Split('/');

            var index = url.IndexOf(route[0]);
            string makeUrl = null;

            if (index != -1)
            {
                makeUrl = url.Substring(index);
            }

            return makeUrl;
        }

        public static string GetUrlForListPermission(HttpRequestData req, string registeredRoute)
        {
            var makeUrl = GetUrlForListPermission(req, registeredRoute);

            makeUrl = string.Join('/', "services", makeUrl);

            return makeUrl;
        }

        public static string GetUrlForPermission(HttpRequestData req, string route, string roleName)
        {
            var makeUrl = GetUrl(req, route);

            if (roleName == "create")
                makeUrl = makeUrl + "/*";

            return makeUrl;
        }
    }
}
