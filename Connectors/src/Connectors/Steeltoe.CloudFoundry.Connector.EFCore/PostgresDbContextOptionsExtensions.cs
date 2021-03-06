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

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Steeltoe.CloudFoundry.Connector.Services;
using System;
using System.Reflection;

namespace Steeltoe.CloudFoundry.Connector.PostgreSql.EFCore
{
    public static class PostgresDbContextOptionsExtensions
    {
        private static string[] postgresEntityAssemblies = new string[] { "Npgsql.EntityFrameworkCore.PostgreSQL" };
        private static string[] postgresEntityTypeNames = new string[] { "Microsoft.EntityFrameworkCore.NpgsqlDbContextOptionsExtensions" };

        public static DbContextOptionsBuilder UseNpgsql(this DbContextOptionsBuilder optionsBuilder, IConfiguration config, object npgsqlOptionsAction = null)
        {
            if (optionsBuilder == null)
            {
                throw new ArgumentNullException(nameof(optionsBuilder));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var connection = GetConnection(config);

            return DoUseNpgsql(optionsBuilder, connection, npgsqlOptionsAction);
        }

        public static DbContextOptionsBuilder UseNpgsql(this DbContextOptionsBuilder optionsBuilder, IConfiguration config, string serviceName, object npgsqlOptionsAction = null)
        {
            if (optionsBuilder == null)
            {
                throw new ArgumentNullException(nameof(optionsBuilder));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (string.IsNullOrEmpty(serviceName))
            {
                throw new ArgumentException(nameof(serviceName));
            }

            var connection = GetConnection(config, serviceName);

            return DoUseNpgsql(optionsBuilder, connection, npgsqlOptionsAction);
        }

        public static DbContextOptionsBuilder<TContext> UseNpgsql<TContext>(this DbContextOptionsBuilder<TContext> optionsBuilder, IConfiguration config, object npgsqlOptionsAction = null)
            where TContext : DbContext
        {
            if (optionsBuilder == null)
            {
                throw new ArgumentNullException(nameof(optionsBuilder));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var connection = GetConnection(config);

            return DoUseNpgsql<TContext>(optionsBuilder, connection, npgsqlOptionsAction);
        }

        public static DbContextOptionsBuilder<TContext> UseNpgsql<TContext>(this DbContextOptionsBuilder<TContext> optionsBuilder, IConfiguration config, string serviceName, object npgsqlOptionsAction = null)
            where TContext : DbContext
        {
            if (optionsBuilder == null)
            {
                throw new ArgumentNullException(nameof(optionsBuilder));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (string.IsNullOrEmpty(serviceName))
            {
                throw new ArgumentException(nameof(serviceName));
            }

            var connection = GetConnection(config, serviceName);

            return DoUseNpgsql<TContext>(optionsBuilder, connection, npgsqlOptionsAction);
        }

        public static MethodInfo FindUseNpgsqlMethod(Type type, Type[] parameterTypes)
        {
            var typeInfo = type.GetTypeInfo();
            var declaredMethods = typeInfo.DeclaredMethods;

            foreach (MethodInfo ci in declaredMethods)
            {
                var parameters = ci.GetParameters();

                if (parameters.Length == 3 && ci.Name.Equals("UseNpgsql") &&
                    parameters[0].ParameterType.Equals(parameterTypes[0]) &&
                    parameters[1].ParameterType.Equals(parameterTypes[1]) &&
                    ci.IsPublic && ci.IsStatic)
                {
                    return ci;
                }
            }

            return null;
        }

        private static string GetConnection(IConfiguration config, string serviceName = null)
        {
            PostgresServiceInfo info = null;
            if (string.IsNullOrEmpty(serviceName))
            {
                info = config.GetSingletonServiceInfo<PostgresServiceInfo>();
            }
            else
            {
                info = config.GetRequiredServiceInfo<PostgresServiceInfo>(serviceName);
            }

            PostgresProviderConnectorOptions postgresConfig = new PostgresProviderConnectorOptions(config);

            PostgresProviderConnectorFactory factory = new PostgresProviderConnectorFactory(info, postgresConfig, null);
            return factory.CreateConnectionString();
        }

        private static DbContextOptionsBuilder DoUseNpgsql(DbContextOptionsBuilder builder, string connection, object npgsqlOptionsAction = null)
        {
            Type extensionType = ConnectorHelpers.FindType(postgresEntityAssemblies, postgresEntityTypeNames);
            if (extensionType == null)
            {
                Console.WriteLine($"Searched in {string.Join(", ", postgresEntityAssemblies)} | for {string.Join(", ", postgresEntityTypeNames)}");
                throw new ConnectorException("Unable to find DbContextOptionsBuilder extension, are you missing Postgres EntityFramework Core assembly");
            }

            MethodInfo useMethod = FindUseNpgsqlMethod(extensionType, new Type[] { typeof(DbContextOptionsBuilder), typeof(string) });
            if (extensionType == null)
            {
                throw new ConnectorException("Unable to find UseNpgsql extension, are you missing Postgres EntityFramework Core assembly");
            }

            object result = ConnectorHelpers.Invoke(useMethod, null, new object[] { builder, connection, npgsqlOptionsAction });
            if (result == null)
            {
                throw new ConnectorException(string.Format("Failed to invoke UseNpgsql extension, connection: {0}", connection));
            }

            return (DbContextOptionsBuilder)result;
        }

        private static DbContextOptionsBuilder<TContext> DoUseNpgsql<TContext>(DbContextOptionsBuilder<TContext> builder, string connection, object npgsqlOptionsAction = null)
            where TContext : DbContext
        {
            return (DbContextOptionsBuilder<TContext>)DoUseNpgsql((DbContextOptionsBuilder)builder, connection, npgsqlOptionsAction);
        }
    }
}
