namespace DataConnectorLibraryProject.Models
{
    internal class Service
    {
        public Guid ServiceId { get; set; }
        public string ServiceName { get; set; }
        public double ServicePrice { get; set; }
        public double ServicePriceWithPdv { get; set; }
        public ICollection<ProvidedService> ProvidedServices { get; set; }
    }
}