﻿// Copyright 2017 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Autofac;
using Autofac.Builder;
using Microsoft.Extensions.Configuration;
using Steeltoe.CloudFoundry.Connector;
using Steeltoe.CloudFoundry.Connector.PostgreSql;
using Steeltoe.CloudFoundry.Connector.Relational;
using Steeltoe.CloudFoundry.Connector.Relational.PostgreSql;
using Steeltoe.CloudFoundry.Connector.Services;
using Steeltoe.Common.HealthChecks;
using System;
using System.Data;

namespace Steeltoe.CloudFoundry.ConnectorAutofac
{
    public static class PostgreSqlContainerBuilderExtensions
    {
        /// <summary>
        /// Adds NpgsqlConnection (as IDbConnection and NpgsqlConnection) to your Autofac Container
        /// </summary>
        /// <param name="container">Your Autofac Container Builder</param>
        /// <param name="config">Application configuration</param>
        /// <param name="serviceName">Cloud Foundry service name binding</param>
        /// <returns>the RegistrationBuilder for (optional) additional configuration</returns>
        public static IRegistrationBuilder<object, SimpleActivatorData, SingleRegistrationStyle> RegisterPostgreSqlConnection(this ContainerBuilder container, IConfiguration config, string serviceName = null)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            PostgresServiceInfo info = serviceName == null
                ? config.GetSingletonServiceInfo<PostgresServiceInfo>()
                : config.GetRequiredServiceInfo<PostgresServiceInfo>(serviceName);

            Type postgreSqlConnection = PostgreSqlTypeLocator.NpgsqlConnection;
            var postgreSqlConfig = new PostgresProviderConnectorOptions(config);
            var factory = new PostgresProviderConnectorFactory(info, postgreSqlConfig, postgreSqlConnection);
            container.RegisterType<RelationalHealthContributor>().As<IHealthContributor>();
            return container.Register(c => factory.Create(null)).As(typeof(IDbConnection), postgreSqlConnection);
        }
    }
}
