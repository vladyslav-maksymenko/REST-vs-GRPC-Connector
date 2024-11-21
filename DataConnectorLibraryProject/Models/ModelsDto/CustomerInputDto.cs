namespace DataConnectorLibraryProject.Models.ModelsDto
{
    public class CustomerInputDto
    {
        public required string CustomerName { get; init; }
        public required string EdpouCode { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required string Patronymic { get; init; }
        public required string PositionId { get; init; }
    }
}