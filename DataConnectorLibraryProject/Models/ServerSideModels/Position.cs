using DataConnectorLibraryProject.Interface;

namespace DataConnectorLibraryProject.Models.ServerSideModels
{
    public class Position : IEntity
    {
        public string Id { get; set; } 
        public string PositionName { get; set; }
        public ICollection<Employee> Employees { get; set; }
        public ICollection<Customer> Customers { get; set; }
    }
}
