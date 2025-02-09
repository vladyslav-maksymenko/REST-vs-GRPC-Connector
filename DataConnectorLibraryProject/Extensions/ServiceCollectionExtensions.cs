using DataConnectorLibraryProject.DataAccess.Data;
using DataConnectorLibraryProject.Interface;
using DataConnectorLibraryProject.Serializers;
using DataConnectorLibraryProject.ServiceInterfaces;
using DataConnectorLibraryProject.Services;
using DataConnectorLibraryProject.Settings.Mongo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataConnectorLibraryProject.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataConnectors(this IServiceCollection services, IConfiguration configuration)
        {
            // SQL Server Configuration.
            services.AddDbContext<SqlDataConnectorDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ConnectionForSqlDb")));

            // MongoDB Configuration.
            MongoDbSerialization.AddCustomMongoDbSerialization();
            var mongoSettings = new MongoDbSettings();
            configuration.GetSection("MongoDbSettings").Bind(mongoSettings);
            //services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings")); 
            services.AddDbContext<MongoDataConnectorDbContext>(options =>
                options.UseMongoDB(mongoSettings.AtlasUri ?? string.Empty, mongoSettings.DatabaseName ?? string.Empty));
            
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            services.AddScoped<ICustomerService, CustomerService>();

            return services;
        }

        public static IServiceCollection AddCustomMapping(this IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            return services;
        }
    }
}