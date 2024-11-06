using DataConnectorLibraryProject.Interface;

namespace DataConnectorLibraryProject.Models
{
    internal class Customer : IEntity
    {
        public Guid Id { get; set; }
        public string CustomerName { get; init; }
        public string EdpouCode { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Patronymic { get; init; }
        public Guid PositionId { get; init; }
        public Position Position { get; set; }
        public ICollection<Employee> Employees { get; set; }
        public ICollection<Vehicle> Vehicles { get; set; }
    }
}
