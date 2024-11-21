using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataConnectorLibraryProject.Interface;

namespace DataConnectorLibraryProject.Models
{
    public class Service : IEntity
    {
        public string Id { get; set; } 
        public string ServiceName { get; set; }
        public double ServicePrice { get; set; }
        public double ServicePriceWithPdv { get; set; }
        public ICollection<ProvidedService> ProvidedServices { get; set; }
    }
}