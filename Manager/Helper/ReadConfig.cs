using Microsoft.Extensions.Configuration;
using System.IO;


namespace Manager.Helper
{
    public class ReadConfig
    {
        private static ReadConfig _appSettings;

        public string appSettingValue { get; set; }

        public ReadConfig(IConfiguration config, string Key)
        {
            this.appSettingValue = config.GetValue<string>(Key);
        }

        private static ReadConfig GetCurrentSettings(string section, string key)
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.AddEnvironmentVariables();
            IConfigurationRoot configuration = builder.Build();
            var settings = new ReadConfig(configuration.GetSection(section), key);
            return settings;
        }


        public static string ConnectionString(string key)
        {
            _appSettings = GetCurrentSettings("ConnectionStrings", key);
            return _appSettings.appSettingValue;
        }
    }
}
