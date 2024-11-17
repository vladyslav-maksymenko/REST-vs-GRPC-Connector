using DataConnectorLibraryProject.Interface;

namespace DataConnectorLibraryProject.Models
{
    public class Employee : IEntity
    {
        public Guid Id { get; set; } 
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