using DataConnectorLibraryProject.Interface;

namespace DataConnectorLibraryProject.Models
{
    internal class Service : IEntity
    {
        public Guid Id { get; set; }
        public string ServiceName { get; set; }
        public double ServicePrice { get; set; }
        public double ServicePriceWithPdv { get; set; }
        public ICollection<ProvidedService> ProvidedServices { get; set; }
    }
}