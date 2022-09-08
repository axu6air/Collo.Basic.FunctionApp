using Microsoft.Extensions.Logging;

namespace Collo.Cloud.Services.Libraries.Shared.Commons
{
    public class LoggerCore
    {
        public static ILogger GetLogger(string categoryName)
        {
            var factory = LoggerFactory.Create(b =>
            {
                b.AddConsole();
                b.AddDebug();
            });

            ILogger logger = factory.CreateLogger(categoryName);

            return logger;
        }

        public static ILogger<T> GetLogger<T>() where T : class
        {
            var factory = LoggerFactory.Create(b =>
            {
                b.AddConsole();
                b.AddDebug();
            });

            ILogger<T> logger = factory.CreateLogger<T>();

            return logger;
        }
    }
}
