namespace DataConnectorLibraryProject.Models
{
    internal class Performer
    {
        public Guid PerformerId { get; set; }
        public string PerfomerName { get; set; }
        public string EdpouCode { get; set; }
        public ICollection<Employee> Employees { get; set; }
        public ICollection<ProvidedService> ProvidedServices { get; set; }
    }
}

