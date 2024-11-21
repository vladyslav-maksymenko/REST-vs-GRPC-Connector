using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataConnectorLibraryProject.Interface;
using MongoDB.Bson.Serialization.Attributes;

namespace DataConnectorLibraryProject.Models
{
    public class Vehicle : IEntity
    {
        public string Id { get; set; } 
        public string PhoneNumber { get; set; }
        public string BrandVehicle { get; set; }
        public DateTime ExecutionDate { get; set; }
        public string StateNumber { get; set; }
        public string TypeVehicleId { get; set; }
        public TypeVehicle TypeVehicle { get; set; }
        public string CustomerId { get; set; }
        [BsonIgnore]
        public Customer Customer { get; set; }
        public ICollection<Equipment> Equipments { get; set; }
        public ICollection<ProvidedService> ProvidedServices { get; set; }
    }
}
