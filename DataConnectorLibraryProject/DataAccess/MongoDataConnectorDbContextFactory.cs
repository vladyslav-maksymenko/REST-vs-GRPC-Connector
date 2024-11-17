/*using DataConnectorLibraryProject.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace DataConnectorLibraryProject.DataAccess
{
    internal class MongoDataConnectorDbContextFactory : IDesignTimeDbContextFactory<MongoDataConnectorDbContext>
    {
        public MongoDataConnectorDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var mongoConnectionString = configuration.GetSection("MongoDBSettings:AtlasURI").Value;
            var mongoClient = new MongoClient(mongoConnectionString);
            
            var database = mongoClient.GetDatabase(configuration["MongoDBSettings:DatabaseName"]);

            var options = new DbContextOptionsBuilder<MongoDataConnectorDbContext>()
                .UseMongoDB(mongoClient, database.DatabaseNamespace.DatabaseName)
                .Options;

            return new MongoDataConnectorDbContext(options);
        }
    }
}*/