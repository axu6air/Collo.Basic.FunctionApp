using Collo.Cloud.Services.Libraries.Shared.Persistence.Data.Entities;

namespace Collo.Cloud.Services.Libraries.Shared.Helpers
{
    internal static class CosmosDbHelper
    {
        public static Dictionary<string, string> GetCosmosContainers()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                 .Where(x => typeof(IContainerEntity).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                 .Select(x => new KeyValuePair<string, string>(x.Name, x.GetProperty("Id").Name.ToString()))
                 .ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
