using DataConnectorLibraryProject.Interface;

namespace DataConnectorLibraryProject.Models
{
    internal class Position : IEntity
    {
        public Guid Id { get; set; }
        public string PositionName { get; set; }
        public ICollection<Employee> Employees { get; set; }
        public ICollection<Customer> Customers { get; set; }
    }
}
