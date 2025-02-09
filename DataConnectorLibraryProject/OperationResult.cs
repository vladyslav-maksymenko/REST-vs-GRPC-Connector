using Microsoft.IdentityModel.Tokens;

namespace DataConnectorLibraryProject
{
    public class OperationResult<TData>
    {
        private readonly List<string> errorMessage = [];

        // Using Count == 0 because errorMessage is a List<string>, which provides O(1) access to Count.  
        // This is more efficient than Any(), which may iterate over the collection in other cases.
        public bool IsSuccess => errorMessage.Count == 0;
        public IReadOnlyCollection<string> ErrorMessages  => errorMessage.AsReadOnly();
        public TData? Result { get; }
        protected OperationResult(TData? resultData = default) => Result = resultData;

        public static OperationResult<TData> Success(TData? resultData = default) => new(resultData);
        
        public static OperationResult<TData> Failure(params string[] errorMessages)
        {
            if (errorMessages.IsNullOrEmpty() || errorMessages.Any(string.IsNullOrEmpty))
            {
                throw new ArgumentException("At least one non-empty error message is required.");
            }
            var result = new OperationResult<TData>();
            result.errorMessage.AddRange(errorMessages);
            return result;
        }
        
        public bool TryGetResult(out TData? result)
        {
            result = IsSuccess ? Result : default;
            return IsSuccess;
        }

        protected OperationResult<TData> AddError(params string[] errorMessages)
        {
            if (errorMessages.IsNullOrEmpty() || errorMessages.Any(string.IsNullOrEmpty))
            {
                throw new ArgumentException("At least one non-empty error message is required.");
            }
            errorMessage.AddRange(errorMessages);
            return this;
        }
    }
}