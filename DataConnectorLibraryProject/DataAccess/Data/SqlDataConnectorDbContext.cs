using DataConnectorLibraryProject.Models.ServerSideModels;
using Microsoft.EntityFrameworkCore;

namespace DataConnectorLibraryProject.DataAccess.Data
{
    public class SqlDataConnectorDbContext : DbContext
    {
        public SqlDataConnectorDbContext(DbContextOptions<SqlDataConnectorDbContext> options) : base(options)
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
            base.OnModelCreating(modelBuilder);
            SetupPrimaryKey(modelBuilder);

            modelBuilder.Entity<Employee>()
                .HasOne(x => x.Position)
                .WithMany(x => x.Employees)
                .HasForeignKey(x => x.PositionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Employee>()
              .HasMany(x => x.Performers)
              .WithMany(x => x.Employees);

            modelBuilder.Entity<Customer>()
                .HasOne(x => x.Position)
                .WithMany(x => x.Customers)
                .HasForeignKey(x => x.PositionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Customer>()
              .HasMany(x => x.Employees)
              .WithMany(x => x.Customers);

            modelBuilder.Entity<Vehicle>()
                .HasOne(x => x.TypeVehicle)
                .WithMany(x => x.Vehicles)
                .HasForeignKey(x => x.TypeVehicleId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Vehicle>()
                .HasOne(x => x.Customer)
                .WithMany(x => x.Vehicles)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Equipment>()
                .HasOne(x => x.Vehicle)
                .WithMany(x => x.Equipments)
                .HasForeignKey(x => x.VehicleId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ProvidedService>()
                .HasOne(x => x.Vehicle)
                .WithMany(x => x.ProvidedServices)
                .HasForeignKey(x => x.VehicleId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Service>()
                .HasMany(x => x.ProvidedServices)
                .WithOne(x => x.Service)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ProvidedService>()
                .HasOne(x => x.Performer)
                .WithMany(x => x.ProvidedServices)
                .HasForeignKey(x => x.PerformerId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Customer>().Ignore(x => x.EmployeeId);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("ConnectionForSqlDb");
            }
        }

        private void SetupPrimaryKey(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().HasKey(c => c.Id);
            modelBuilder.Entity<Employee>().HasKey(e => e.Id);
            modelBuilder.Entity<Performer>().HasKey(p => p.Id);
            modelBuilder.Entity<Position>().HasKey(p => p.Id);
            modelBuilder.Entity<ProvidedService>().HasKey(ps => ps.Id);
            modelBuilder.Entity<Service>().HasKey(s => s.Id);
            modelBuilder.Entity<TypeVehicle>().HasKey(tv => tv.Id);
            modelBuilder.Entity<Vehicle>().HasKey(v => v.Id);
            modelBuilder.Entity<Equipment>().HasKey(eq => eq.Id);
        }
    }
}