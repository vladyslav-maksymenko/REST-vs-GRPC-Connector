namespace WebApiProject.ModelsDTO
{
    public record ResponseDTO<T>(T Data, ExecutionMetrics Metrics) where T : class;
}
