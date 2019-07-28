using Microsoft.Extensions.Configuration;

namespace AzureStorageQueue.Common
{
    public class ConfigurationHanlder
    {
        private static AppOptions _appOptions;
        private static void BuildConfiguration()
        {

            if (_appOptions == null)
            {
                var builder = new ConfigurationBuilder();
                builder.AddJsonFile("appsettings.json", true, true);

                var settingsCache = builder.Build();
                _appOptions = new AppOptions();
                settingsCache.Bind(_appOptions);
            }

        }

        public static AppOptions AppOptions()
        {
            if (_appOptions == null)
            {
                BuildConfiguration();
            }

            return _appOptions;
        }

    }
}