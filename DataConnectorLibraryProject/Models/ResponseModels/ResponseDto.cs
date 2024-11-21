namespace DataConnectorLibraryProject.Models.ResponseModels
{
    public record ResponseDto<T>(T Data, ExecutionMetrics Metrics) 
        where T : class;
}