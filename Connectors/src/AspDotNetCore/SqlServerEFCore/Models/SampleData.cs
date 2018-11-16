using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace SqlServerEFCore
{
    public class SampleData
    {
        internal static void InitializeMyContext1(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException("serviceProvider");
            }
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db1 = serviceScope.ServiceProvider.GetService<Test1Context>();
                db1.Database.EnsureCreated();
            }
            InitializeContext1(serviceProvider);
        }

        internal static void InitializeMyContext2(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException("serviceProvider");
            }

            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db2 = serviceScope.ServiceProvider.GetService<Test2Context>();
                db2.Database.EnsureCreated();
            }
            InitializeContext2(serviceProvider);
        }

        private static void InitializeContext1(IServiceProvider serviceProvider)
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db1 = serviceScope.ServiceProvider.GetService<Test1Context>();
                if (db1.Test1Data.Any())
                {
                    return;
                }

                AddData<Test1Data>(db1, new Test1Data() { Id = 1, Data = "Test Data 1 - Test1Context " });
                AddData<Test1Data>(db1, new Test1Data() { Id = 2, Data = "Test Data 2 - Test1Context " });
                db1.SaveChanges();
            }
        }

        private static void InitializeContext2(IServiceProvider serviceProvider)
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db2 = serviceScope.ServiceProvider.GetService<Test2Context>();
                if (db2.Test2Data.Any())
                {
                    return;
                }

                AddData<Test2Data>(db2, new Test2Data() { Id = 1, Data = "Test Data 1 - Test2Context " });
                AddData<Test2Data>(db2, new Test2Data() { Id = 2, Data = "Test Data 2 - Test2Context " });
                db2.SaveChanges();
            }
        }

        private static void AddData<TData>(DbContext db, object item) where TData: class
        {
            db.Entry(item).State = EntityState.Added;
        }
    }
}
