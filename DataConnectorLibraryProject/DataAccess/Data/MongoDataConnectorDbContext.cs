using DataConnectorLibraryProject.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace DataConnectorLibraryProject.DataAccess.Data
{
    public class MongoDataConnectorDbContext : DbContext
    {
        public MongoDataConnectorDbContext(DbContextOptions<MongoDataConnectorDbContext> options) : base(options)
        {
        }
        
        internal DbSet<Customer> Customers { get; set; }
        internal DbSet<Employee> Employees { get; set; }
        internal DbSet<Equipment> Equipments { get; set; }
        internal DbSet<Performer> Performers { get; set; }
        internal DbSet<Position> Positions { get; set; }
        internal DbSet<ProvidedService> ProvidedServices { get; set; }
        internal DbSet<Service> Services { get; set; }
        internal DbSet<TypeVehicle> TypeVehicles { get; set; }
        internal DbSet<Vehicle> Vehicles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().ToCollection("customer");
            modelBuilder.Entity<Employee>().ToCollection("employee");
            modelBuilder.Entity<Equipment>().ToCollection("equipment");
            modelBuilder.Entity<Performer>().ToCollection("performer");
            modelBuilder.Entity<Position>().ToCollection("position");
            modelBuilder.Entity<ProvidedService>().ToCollection("providedService");
            modelBuilder.Entity<Service>().ToCollection("service");
            modelBuilder.Entity<TypeVehicle>().ToCollection("typeVehicle");
            modelBuilder.Entity<Vehicle>().ToCollection("vehicle");
        }
    }
}
