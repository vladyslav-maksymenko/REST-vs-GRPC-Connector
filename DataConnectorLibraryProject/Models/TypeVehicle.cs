using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataConnectorLibraryProject.Interface;

namespace DataConnectorLibraryProject.Models
{
    public class TypeVehicle : IEntity
    {
        public string Id { get; set; } 
        public string CustomerName { get; set; }
        public ICollection<Vehicle> Vehicles { get; set; }
    }
}
