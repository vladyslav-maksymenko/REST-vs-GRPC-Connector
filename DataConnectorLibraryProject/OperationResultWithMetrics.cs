using DataConnectorLibraryProject.Models.ResponseModels;

namespace DataConnectorLibraryProject
{
    public class OperationResultWithMetrics<TData> : OperationResult<TData>
    {
        public ExecutionMetrics? Metrics { get; private set; }

        private OperationResultWithMetrics(TData? data, ExecutionMetrics? metrics) : base(data)
        {
            Metrics = metrics;
        }
        
        public static OperationResultWithMetrics<TData> WithDataAndMetrics(
            TData? data = default, ExecutionMetrics? metrics = null) => new(data, metrics);

        public static OperationResultWithMetrics<TData> Failure(string error)
        {
            ArgumentException.ThrowIfNullOrEmpty(error);
            var result = new OperationResultWithMetrics<TData>(default, null);
            result.AddError(error);
            return result;
        }
    }
}