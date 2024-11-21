using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataConnectorLibraryProject.Interface;

namespace DataConnectorLibraryProject.Models
{
    public class Position : IEntity
    {
        public string Id { get; set; } 
        public string PositionName { get; set; }
        public ICollection<Employee> Employees { get; set; }
        public ICollection<Customer> Customers { get; set; }
    }
}
