using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataConnectorLibraryProject.Interface;

namespace DataConnectorLibraryProject.Models
{
    public class ProvidedService : IEntity
    {
        public string Id { get; set; } 
        public string ServiceId { get; set; }
        public Service Service { get; set; }
        public string PerformerId { get; set; }
        public Performer Performer { get; set; }
        public string VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }
        public DateTime ExecutionDate { get; set; }
    }
}
