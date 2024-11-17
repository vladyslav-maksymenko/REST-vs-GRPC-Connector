using DataConnectorLibraryProject.Interface;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;

namespace DataConnectorLibraryProject.Models
{
    [Collection("Customer")]
    public class Customer : IEntity
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public string EdpouCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public Guid PositionId { get; set; }

        public Guid? EmployeeId { get; set; }
        [BsonIgnore]
        public Position Position { get; set; }
        [BsonIgnore]
        public ICollection<Employee> Employees { get; set; }
        [BsonIgnore]
        public ICollection<Vehicle> Vehicles { get; set; }
    }
}
