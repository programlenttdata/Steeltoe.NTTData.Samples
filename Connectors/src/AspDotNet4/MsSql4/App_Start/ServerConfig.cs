using Microsoft.Extensions.Configuration;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using System;
using System.IO;

namespace MsSql4
{
    public static class ServerConfig
    {
        // Multi-DB connection only seem to work when we add them to environment variable, SteelToe can't seem to acknowledge appSettings.json.
        // This is for debug purposes only and 
        // Run the docker-compose file to spin up the SQL Server instances.
        private const string MULTIPLE_SQLSERVER_DB = @"
            {
                'SqlServer': [
                    {
                        'name': 'mySqlServerService1',
                        'credentials': {
                            'ConnectionString': 'Server=tcp:localhost,5433;Initial Catalog=SteeltoeEF1;User Id=sa;Password=Pass@word',
                            'uid': 'sa',
                            'uri': 'jdbc:sqlserver://localhost:5433;databaseName=SteeltoeEF1',
                            'db': 'SteeltoeEFCore1',
                            'pw': 'Pass@word'
                        },
                        'label': 'sqlserver',
                        'tags': [
                            'sqlserver'
                        ]
                    },
                    {
                        'name': 'mySqlServerService2',
                        'credentials': {
                            'ConnectionString': 'Server=tcp:localhost,5434;Initial Catalog=SteeltoeEF2;User Id=sa;Password=Pass@word',
                            'uid': 'sa',
                            'uri': 'jdbc:sqlserver://localhost:5434;databaseName=SteeltoeEF2',
                            'db': 'SteeltoeEFCore2',
                            'pw': 'Pass@word'
                        },
                        'label': 'sqlserver',
                        'tags': [
                            'sqlserver'
                        ]
                    }
                ]
            }";

        public static IConfigurationRoot Configuration { get; set; }

        public static void RegisterConfig(string environment)
        {
            Environment.SetEnvironmentVariable("VCAP_SERVICES", MULTIPLE_SQLSERVER_DB);

            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(GetContentRoot())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddCloudFoundry()
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }
        public static string GetContentRoot()
        {
            var basePath = (string)AppDomain.CurrentDomain.GetData("APP_CONTEXT_BASE_DIRECTORY") ??
               AppDomain.CurrentDomain.BaseDirectory;
            return Path.GetFullPath(basePath);
        }
    }
}