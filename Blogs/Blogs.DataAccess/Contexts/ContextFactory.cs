using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Data.Sqlite;

namespace Blogs.DataAccess.Contexts
{
    public enum ContextType {SqLite = 0, SqlServer = 1}

    [ExcludeFromCodeCoverage]
    public class ContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            return GetNewContext();
        }

        public static DataContext GetNewContext(ContextType type = ContextType.SqlServer)
        {
            var builder = new DbContextOptionsBuilder<DataContext>();
            if (type == ContextType.SqLite)
            {
                var connection = new SqliteConnection("Filename=:memory:");
                builder.UseSqlite(connection);
            }
            else
            {
                //type = ContextType.SqlServer
                var connectionString = GetSqlConnectionString();
                builder.UseSqlServer(connectionString);
            }

            return new DataContext(builder.Options);
        }

        private static string GetSqlConnectionString()
        {
            string basePath = Directory.GetCurrentDirectory();
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(basePath)
            .AddJsonFile("appsettings.json").Build();

            return configuration.GetConnectionString(@"BlogsDB");
        }

    }
}
