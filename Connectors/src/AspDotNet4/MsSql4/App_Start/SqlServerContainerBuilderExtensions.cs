using Microsoft.Extensions.Configuration;
using MsSql4.Data;
using Steeltoe.CloudFoundry.Connector;
using Steeltoe.CloudFoundry.Connector.Services;
using Steeltoe.CloudFoundry.Connector.SqlServer;
using StructureMap;
using System.Data.SqlClient;

namespace MsSql4
{
    public static class SqlServerContainerBuilderExtensions
    {
        public static void RegisterSqlServerConnection(IContainer container, IConfigurationRoot config)
        {
            SqlServerProviderConnectorOptions SqlServerConfig = new SqlServerProviderConnectorOptions(config);

            SqlServerServiceInfo info1 = config.GetRequiredServiceInfo<SqlServerServiceInfo>("mySqlServerService1"); // config.GetSingletonServiceInfo<SqlServerServiceInfo>();
            SqlServerProviderConnectorFactory factory1 = new SqlServerProviderConnectorFactory(info1, SqlServerConfig, typeof(SqlConnection));
            container.Inject<IBloggingContext>(new BloggingContext((SqlConnection)factory1.Create(null)));

            SqlServerServiceInfo info2 = config.GetRequiredServiceInfo<SqlServerServiceInfo>("mySqlServerService2"); // config.GetSingletonServiceInfo<SqlServerServiceInfo>();
            SqlServerProviderConnectorFactory factory2 = new SqlServerProviderConnectorFactory(info2, SqlServerConfig, typeof(SqlConnection));
            container.Inject<IVloggingContext>(new VloggingContext((SqlConnection)factory2.Create(null)));
        }
    }
}