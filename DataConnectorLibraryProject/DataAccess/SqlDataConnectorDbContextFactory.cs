using DataConnectorLibraryProject.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DataConnectorLibraryProject.DataAccess
{
    internal class SqlDataConnectorDbContextFactory : IDesignTimeDbContextFactory<SqlDataConnectorDbContext>
    {
        public SqlDataConnectorDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqlDataConnectorDbContext>();

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

            optionsBuilder.UseSqlServer(configuration.GetConnectionString("ConnectionForSqlDb"));

            return new SqlDataConnectorDbContext(optionsBuilder.Options);
        }
    }
}
