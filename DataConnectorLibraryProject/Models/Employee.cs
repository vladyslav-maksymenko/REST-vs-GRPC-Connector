namespace DataConnectorLibraryProject.Models
{
    internal class Employee
    {
        public Guid EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string IpnCode { get; set; }
        public Guid PositionId { get; set; }
        public Position Position { get; set; }
        public ICollection<Performer> Performers { get; set; }
        public ICollection<Customer> Customers { get; set; }
    }
}