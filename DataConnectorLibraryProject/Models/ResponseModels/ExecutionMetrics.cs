namespace DataConnectorLibraryProject.Models.ResponseModels
{
    public record ExecutionMetrics(
        string? SqlQueryTime = null,
        string? MongoQueryTime = null,
        string? TotalExecutionTime = null);
}