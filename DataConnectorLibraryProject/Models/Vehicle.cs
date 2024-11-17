using DataConnectorLibraryProject.Interface;
using MongoDB.Bson.Serialization.Attributes;

namespace DataConnectorLibraryProject.Models
{
    public class Vehicle : IEntity
    {
        public Guid Id { get; set; } 
        public string PhoneNumber { get; set; }
        public string BrandVehicle { get; set; }
        public DateTime ExecutionDate { get; set; }
        public string StateNumber { get; set; }
        public Guid TypeVehicleId { get; set; }
        public TypeVehicle TypeVehicle { get; set; }
        public Guid CustomerId { get; set; }
        [BsonIgnore]
        public Customer Customer { get; set; }
        public ICollection<Equipment> Equipments { get; set; }
        public ICollection<ProvidedService> ProvidedServices { get; set; }
    }
}
