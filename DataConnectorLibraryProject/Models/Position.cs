namespace DataConnectorLibraryProject.Models
{
    internal class Position
    {
        public Guid PositionId { get; set; }
        public string PositionName { get; set; }
        public ICollection<Employee> Employees { get; set; }
        public ICollection<Customer> Customers { get; set; }
    }
}
