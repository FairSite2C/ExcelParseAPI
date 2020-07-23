using Microsoft.Extensions.Configuration;
using System.IO;

namespace OriginsRx.Azure
{
    public class AzureStorageConfig
    {
        public string AccountName { get; set; }

        public string AccountKey { get; set; }

    }

    public static class AppSettings
    {
        private static readonly string _connectionString = string.Empty;


        public static IConfigurationRoot appSettings;

        public static AzureStorageConfig Storage;

        static AppSettings()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);

            appSettings = configurationBuilder.Build();
            Storage = new AzureStorageConfig() {
                AccountName =
                appSettings.GetSection("AzureStorageConfig")
                           .GetSection("AccountName").Value,
                AccountKey =
                appSettings.GetSection("AzureStorageConfig")
                            .GetSection("AccountKey").Value
            };
        }

/*
        public static string ConnectionString
        {
            get => _connectionString;
        }
*/

    }
}