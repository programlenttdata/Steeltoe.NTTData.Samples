using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using System;
using System.IO;

namespace SqlServerEFCore
{
    public class Program
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
                            'ConnectionString': 'Server=tcp:localhost,5433;Initial Catalog=SteeltoeEFCore1;User Id=sa;Password=Pass@word',
                            'uid': 'sa',
                            'uri': 'jdbc:sqlserver://localhost:5433;databaseName=SteeltoeEFCore1',
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
                            'ConnectionString': 'Server=tcp:localhost,5434;Initial Catalog=SteeltoeEFCore2;User Id=sa;Password=Pass@word',
                            'uid': 'sa',
                            'uri': 'jdbc:sqlserver://localhost:5434;databaseName=SteeltoeEFCore2',
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

        public static void Main(string[] args)
        {
            // Set VCAP_SERVICES environment variable to simulate services that will be bound in PAS, Look at the constant MULTIPLE_SQLSERVER_DB for illustration.
            Environment.SetEnvironmentVariable("VCAP_SERVICES", MULTIPLE_SQLSERVER_DB);

            var host = BuildWebHost(args);
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    SampleData.InitializeMyContext1(services);
                    SampleData.InitializeMyContext2(services);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return new WebHostBuilder()
                .UseKestrel()
                .UseCloudFoundryHosting()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((builderContext, configBuilder) =>
                {
                    var env = builderContext.HostingEnvironment;
                    configBuilder.SetBasePath(env.ContentRootPath)
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                        .AddEnvironmentVariables()
                        // Add to configuration the Cloudfoundry VCAP settings
                        .AddCloudFoundry();
                })
                .ConfigureLogging((context, builder) =>
                {
                    builder.AddConfiguration(context.Configuration.GetSection("Logging"));
                    builder.AddConsole();
                })
                .Build();
        }
    }
}
