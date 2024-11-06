namespace DataConnectorLibraryProject.Models
{
    internal class TypeVehicle
    {
        public Guid TypeVehicleId { get; set; }
        public string CustomerName { get; set; }
        public ICollection<Vehicle> Vehicles { get; set; }
    }
}
