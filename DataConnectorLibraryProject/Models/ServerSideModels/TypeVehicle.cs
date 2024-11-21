using DataConnectorLibraryProject.Interface;

namespace DataConnectorLibraryProject.Models.ServerSideModels
{
    public class TypeVehicle : IEntity
    {
        public string Id { get; set; } 
        public string CustomerName { get; set; }
        public ICollection<Vehicle> Vehicles { get; set; }
    }
}
