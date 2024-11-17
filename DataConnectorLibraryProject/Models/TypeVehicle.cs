using DataConnectorLibraryProject.Interface;

namespace DataConnectorLibraryProject.Models
{
    public class TypeVehicle : IEntity
    {
        public Guid Id { get; set; } 
        public string CustomerName { get; set; }
        public ICollection<Vehicle> Vehicles { get; set; }
    }
}
