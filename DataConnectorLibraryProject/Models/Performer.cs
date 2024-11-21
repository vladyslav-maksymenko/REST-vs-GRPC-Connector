using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataConnectorLibraryProject.Interface;

namespace DataConnectorLibraryProject.Models
{
    public class Performer : IEntity
    {
        public string Id { get; set; } 
        public string PerfomerName { get; set; }
        public string EdpouCode { get; set; }
        public ICollection<Employee> Employees { get; set; }
        public ICollection<ProvidedService> ProvidedServices { get; set; }
    }
}

